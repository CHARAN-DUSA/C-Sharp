import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  mode         = signal<'email' | 'phone' | '2fa'>('email');
  showPwd      = signal(false);
  showPhonePwd = signal(false);
  loading      = signal(false);
  error        = signal('');
  twoFaUserId  = signal('');

  emailForm = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  phoneLoginForm = this.fb.group({
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\+[1-9]\d{7,14}$/)]],
    password:    ['', [Validators.required, Validators.minLength(6)]]
  });

  twoFaForm = this.fb.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  constructor(
    private fb:    FormBuilder,
    private auth:  AuthService,
    private http:  HttpClient,
    private route: ActivatedRoute   // ✅ inject ActivatedRoute
  ) {}

  // ✅ On init — check if we were redirected here from OAuth with 2FA pending
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['twoFa'] === 'true' && params['userId']) {
        console.log('🔵 2FA required for userId:', params['userId']);
        this.twoFaUserId.set(params['userId']);
        this.mode.set('2fa');   // ✅ switch straight to 2FA screen
      }

      if (params['error']) {
        const messages: Record<string, string> = {
          no_token:      'Google sign-in failed — no token received.',
          state_mismatch:'Security check failed. Please try again.',
          google_failed: 'Google sign-in failed. Please try again.',
        };
        this.error.set(messages[params['error']] ?? 'Sign-in failed. Please try again.');
      }
    });
  }

  // ── Email login ────────────────────────────────────────────
  submitEmail() {
    if (this.emailForm.invalid) { this.emailForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.login(this.emailForm.value as any).subscribe({
      next: (r: any) => {
        this.loading.set(false);
        if (r.requiresTwoFactor) { this.twoFaUserId.set(r.userId); this.mode.set('2fa'); }
        else this.auth.redirectAfterLogin();
      },
      error: (e: any) => {
        this.loading.set(false);
        if (e.status === 429) this.error.set('Too many attempts. Please wait before trying again.');
        else this.error.set(e.error?.detail ?? 'Login failed. Check your credentials.');
      }
    });
  }

  // ── Phone + password login ─────────────────────────────────
  submitPhoneLogin() {
    if (this.phoneLoginForm.invalid) { this.phoneLoginForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.loginWithPhone(
      this.phoneLoginForm.value.phoneNumber!,
      this.phoneLoginForm.value.password!
    ).subscribe({
      next: (r: any) => {
        this.loading.set(false);
        if (r.requiresTwoFactor) { this.twoFaUserId.set(r.userId); this.mode.set('2fa'); }
        else this.auth.redirectAfterLogin();
      },
      error: (e: any) => {
        this.loading.set(false);
        if (e.status === 429) this.error.set('Too many attempts. Please wait.');
        else this.error.set(e.error?.detail ?? 'Login failed. Check your phone and password.');
      }
    });
  }

  // ── Google OAuth ───────────────────────────────────────────
  signInWithGoogle() {
    this.error.set('');
    this.loading.set(true);

    const state = crypto.randomUUID
      ? crypto.randomUUID()
      : Math.random().toString(36).substring(2) + Math.random().toString(36).substring(2);

    localStorage.removeItem('oauth_state');
    localStorage.removeItem('oauth_intended_role');

    setTimeout(() => {
      localStorage.setItem('oauth_state', state);
      localStorage.setItem('oauth_intended_role', 'Patient');

      const redirectUri = `${window.location.origin}/auth/callback`;

      window.location.href =
        `https://accounts.google.com/o/oauth2/v2/auth`
        + `?client_id=${encodeURIComponent(environment.googleClientId)}`
        + `&redirect_uri=${encodeURIComponent(redirectUri)}`
        + `&response_type=token%20id_token`
        + `&scope=${encodeURIComponent('openid email profile')}`
        + `&state=${encodeURIComponent(state)}`
        + `&nonce=${encodeURIComponent(crypto.randomUUID
            ? crypto.randomUUID()
            : Math.random().toString(36).substring(2))}`
        + `&prompt=select_account`;
    }, 50);
  }

  // ── 2FA verify ─────────────────────────────────────────────
  verifyTwoFa() {
    if (this.twoFaForm.invalid) { this.twoFaForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.verifyTwoFactor(this.twoFaUserId(), this.twoFaForm.value.code!).subscribe({
      next: () => { this.loading.set(false); this.auth.redirectAfterLogin(); },
      error: (e: any) => {
        this.loading.set(false);
        this.error.set(e.error?.detail ?? 'Invalid 2FA code.');
      }
    });
  }

  togglePassword()      { this.showPwd.update(v => !v); }
  togglePhonePassword() { this.showPhonePwd.update(v => !v); }
}