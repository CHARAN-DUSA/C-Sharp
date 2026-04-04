import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FirebaseService } from '../../../core/services/firebase.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit
{

  // ── State ──────────────────────────────────────────────────────────────────
  method = signal<'email' | 'phone' | 'google'>('email');
  step = signal<'input' | 'otp' | 'newPassword' | 'success'>('input');
  loading = signal(false);
  error = signal('');
  resendCooldown = signal(0);

  // ── Forms ──────────────────────────────────────────────────────────────────
  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  phoneForm = this.fb.group({
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\+[1-9]\d{7,14}$/)]]
  });

  otpForm = this.fb.group({
    otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  // ✅ New password form after OTP verified
  newPasswordForm = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private firebase: FirebaseService
  ) { }

  ngOnInit()
  {
    setTimeout(() =>
    {
      this.firebase.setupRecaptcha('recaptcha-container');
    }, 500);
  }

  // ── Method 1: Email OTP via YOUR BACKEND ─────────────────────────────────
  sendEmailOtp()
  {
    if (this.emailForm.invalid) { this.emailForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');

    // ✅ Calls your backend — not Firebase
    this.http.post(
      `${environment.apiUrl}/auth/forgot-password/send-otp`,
      { email: this.emailForm.value.email }
    ).subscribe({
      next: () =>
      {
        this.loading.set(false);
        this.step.set('otp');
        this.startResendCooldown();
      },
      error: (e: any) =>
      {
        this.loading.set(false);
        this.error.set(e.error?.message ?? e.error?.detail ?? 'Failed to send OTP. Check the address.');
      }
    });
  }

  // ✅ Verify email OTP + set new password via YOUR BACKEND
  verifyEmailOtp()
  {
    if (this.otpForm.invalid || this.newPasswordForm.invalid)
    {
      this.otpForm.markAllAsTouched();
      this.newPasswordForm.markAllAsTouched();
      return;
    }

    const pw = this.newPasswordForm.value.newPassword!;
    const cpw = this.newPasswordForm.value.confirmPassword!;

    if (pw !== cpw)
    {
      this.error.set('Passwords do not match.');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.http.post(
      `${environment.apiUrl}/auth/forgot-password/verify-otp`,
      {
        email: this.emailForm.value.email,
        otp: this.otpForm.value.otp,
        newPassword: pw
      }
    ).subscribe({
      next: () =>
      {
        this.loading.set(false);
        this.step.set('success');
      },
      error: (e: any) =>
      {
        this.loading.set(false);
        this.error.set(e.error?.detail ?? 'Invalid or expired OTP.');
      }
    });
  }

  resendEmailOtp()
  {
    if (this.resendCooldown() > 0) return;
    this.step.set('input');
    this.otpForm.reset();
    this.sendEmailOtp();
  }

  // ── Method 2: Phone OTP via Firebase ──────────────────────────────────────
  async sendPhoneOtp()
  {
    if (this.phoneForm.invalid) { this.phoneForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    try
    {
      await this.firebase.sendPhoneOtp(this.phoneForm.value.phoneNumber!);
      this.step.set('otp');
      this.startResendCooldown();
      // In sendOtp / sendPhoneOtp catch blocks — add console.error
    } catch (e: any)
    {
      console.error('❌ OTP error:', e.code, e.message);
      const msg = e.code === 'auth/billing-not-enabled'
        ? 'Phone OTP is temporarily unavailable. Please use email login.'
        : e.code === 'auth/invalid-phone-number'
          ? 'Invalid phone number. Use format: +919876543210'
          : e.code === 'auth/too-many-requests'
            ? 'Too many requests. Please wait a few minutes.'
            : e.code === 'auth/captcha-check-failed'
              ? 'reCAPTCHA failed. Please refresh the page and try again.'
              : e.code === 'auth/quota-exceeded'
                ? 'SMS quota exceeded. Please try again tomorrow.'
                : `Failed to send OTP. Please try again. (${e.code})`;
      this.error.set(msg);
    }
  }

  async verifyPhoneOtp()
  {
    if (this.otpForm.invalid) { this.otpForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    try
    {
      await this.firebase.verifyPhoneOtp(this.otpForm.value.otp!);
      this.step.set('success');
    } catch (e: any)
    {
      const msg = e.code === 'auth/invalid-verification-code'
        ? 'Invalid OTP. Please check and try again.'
        : e.code === 'auth/code-expired'
          ? 'OTP has expired. Please request a new one.'
          : 'Verification failed. Try again.';
      this.error.set(msg);
    } finally
    {
      this.loading.set(false);
    }
  }

  async resendPhoneOtp()
  {
    if (this.resendCooldown() > 0) return;
    this.firebase.resetOtpState();
    this.firebase.setupRecaptcha('recaptcha-container');
    this.step.set('input');
    await this.sendPhoneOtp();
  }

  // ── Method 3: Google Sign-In ───────────────────────────────────────────────
  async signInWithGoogle()
  {
    this.loading.set(true);
    this.error.set('');
    try
    {
      await this.firebase.signInWithGoogle();
      this.step.set('success');
    } catch (e: any)
    {
      const msg = e.code === 'auth/popup-closed-by-user'
        ? 'Sign-in was cancelled.'
        : 'Google sign-in failed. Try again.';
      this.error.set(msg);
    } finally
    {
      this.loading.set(false);
    }
  }

  // ── Helpers ────────────────────────────────────────────────────────────────
  changeMethod(m: 'email' | 'phone' | 'google')
  {
    this.method.set(m);
    this.step.set('input');
    this.error.set('');
    this.firebase.resetOtpState();
    if (m !== 'email') this.emailForm.reset();
    if (m !== 'phone') this.phoneForm.reset();
    this.otpForm.reset();
    this.newPasswordForm.reset();
  }

  private startResendCooldown()
  {
    this.resendCooldown.set(60);
    const t = setInterval(() =>
    {
      this.resendCooldown.update(v =>
      {
        if (v <= 1) { clearInterval(t); return 0; }
        return v - 1;
      });
    }, 1000);
  }
}