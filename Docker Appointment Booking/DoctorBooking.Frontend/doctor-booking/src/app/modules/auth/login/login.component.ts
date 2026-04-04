import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

declare const google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent
{
  mode = signal<'email' | 'phone' | '2fa'>('email');
  showPwd = signal(false);
  showPhonePwd = signal(false);
  loading = signal(false);
  error = signal('');
  twoFaUserId = signal('');

  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  // ✅ Phone login now uses phone + password
  phoneLoginForm = this.fb.group({
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\+[1-9]\d{7,14}$/)]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  twoFaForm = this.fb.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  private googleInitialized = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private http: HttpClient
  ) { }

  // ── Email login ───────────────────────────────────────────
  submitEmail()
  {
    if (this.emailForm.invalid) { this.emailForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.login(this.emailForm.value as any).subscribe({
      next: (r: any) =>
      {
        this.loading.set(false);
        if (r.requiresTwoFactor) { this.twoFaUserId.set(r.userId); this.mode.set('2fa'); }
        else this.auth.redirectAfterLogin();
      },
      error: (e: any) =>
      {
        this.loading.set(false);
        if (e.status === 429) this.error.set('Too many attempts. Please wait before trying again.');
        else this.error.set(e.error?.detail ?? 'Login failed. Check your credentials.');
      }
    });
  }

  // ✅ Phone + password login
  submitPhoneLogin()
  {
    if (this.phoneLoginForm.invalid) { this.phoneLoginForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');

    // ✅ Use service method — no need to touch store() visibility
    this.auth.loginWithPhone(
      this.phoneLoginForm.value.phoneNumber!,
      this.phoneLoginForm.value.password!
    ).subscribe({
      next: (r: any) =>
      {
        this.loading.set(false);
        if (r.requiresTwoFactor)
        {
          this.twoFaUserId.set(r.userId);
          this.mode.set('2fa');
        } else
        {
          this.auth.redirectAfterLogin();
        }
      },
      error: (e: any) =>
      {
        this.loading.set(false);
        if (e.status === 429) this.error.set('Too many attempts. Please wait.');
        else this.error.set(e.error?.detail ?? 'Login failed. Check your phone and password.');
      }
    });
  }

  // ── Google OAuth ──────────────────────────────────────────
  signInWithGoogle()
  {
    if (this.googleInitialized) return;
    this.googleInitialized = true;
    this.error.set('');

    const state = Math.random().toString(36).substring(2);
    localStorage.setItem('oauth_state', state);

    window.location.href =
      `https://accounts.google.com/o/oauth2/v2/auth`
      + `?client_id=${environment.googleClientId}`
      + `&redirect_uri=${encodeURIComponent('http://localhost:4200/auth/callback')}`
      + `&response_type=token%20id_token`
      + `&scope=${encodeURIComponent('openid email profile')}`
      + `&state=${state}`
      + `&nonce=${Math.random().toString(36).substring(2)}`;
  }

  // ── 2FA ───────────────────────────────────────────────────
  verifyTwoFa()
  {
    if (this.twoFaForm.invalid) { this.twoFaForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.verifyTwoFactor(this.twoFaUserId(), this.twoFaForm.value.code!).subscribe({
      next: () => { this.loading.set(false); this.auth.redirectAfterLogin(); },
      error: (e: any) => { this.loading.set(false); this.error.set(e.error?.detail ?? 'Invalid 2FA code.'); }
    });
  }

  togglePassword() { this.showPwd.update(v => !v); }
  togglePhonePassword() { this.showPhonePwd.update(v => !v); }
}