import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, from, switchMap, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginDto, RegisterDto, UserRole } from '../models/user.model';

import { initializeApp, getApps } from 'firebase/app';
import {
  getAuth,
  RecaptchaVerifier,
  signInWithPhoneNumber,
  ConfirmationResult
} from 'firebase/auth';

if (!getApps().length) {
  initializeApp(environment.firebaseConfig);
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'mb_token';
  private readonly USER_KEY  = 'mb_user';
  private _user = signal<AuthResponse | null>(this.loadUser());

  readonly currentUser = this._user.asReadonly();
  readonly isLoggedIn  = computed(() => !!this._user());
  readonly userRole    = computed((): UserRole | null => this._user()?.role ?? null);
  readonly isPatient   = computed(() => this._user()?.role === 'Patient');
  readonly isDoctor    = computed(() => this._user()?.role === 'Doctor');
  readonly isAdmin     = computed(() => this._user()?.role === 'Admin');

  private confirmationResult: ConfirmationResult | null = null;
  private recaptchaVerifier:  RecaptchaVerifier  | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  // ── EMAIL / PASSWORD ───────────────────────────────────────
  login(dto: LoginDto) {
    return this.http.post<any>(`${environment.apiUrl}/auth/login`, dto)
      .pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  register(dto: RegisterDto) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, dto)
      .pipe(tap(r => this.store(r)));
  }

  // ── GOOGLE OAUTH ───────────────────────────────────────────
  googleLogin(idToken: string, role: string = 'Patient') {
    return this.http.post<any>(`${environment.apiUrl}/auth/google`, { idToken, role })
      .pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  // ── PHONE + PASSWORD ───────────────────────────────────────
  loginWithPhone(phoneNumber: string, password: string) {
    return this.http.post<any>(
      `${environment.apiUrl}/auth/login-phone`, { phoneNumber, password }
    ).pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  // ── PHONE OTP VIA FIREBASE ─────────────────────────────────
  sendOtp(phoneNumber: string, role: string = 'Patient'): Observable<void> {
    const auth = getAuth();

    if (this.recaptchaVerifier) {
      try { this.recaptchaVerifier.clear(); } catch (e) { console.warn('reCAPTCHA clear:', e); }
      this.recaptchaVerifier = null;
    }

    const container = document.getElementById('recaptcha-container');
    if (!container) {
      return new Observable(o =>
        o.error({ error: { detail: 'Recaptcha container not found.' } })
      );
    }

    this.recaptchaVerifier = new RecaptchaVerifier(
      auth, 'recaptcha-container', { size: 'invisible' }
    );

    return from(
      signInWithPhoneNumber(auth, phoneNumber, this.recaptchaVerifier)
        .then(result => { this.confirmationResult = result; })
    );
  }

  verifyOtp(phoneNumber: string, otpCode: string, role: string = 'Patient'): Observable<any> {
    if (!this.confirmationResult)
      return new Observable(o =>
        o.error({ error: { detail: 'No OTP request found. Please resend.' } })
      );

    return from(
      this.confirmationResult.confirm(otpCode).then(r => r.user.getIdToken())
    ).pipe(
      switchMap(idToken =>
        this.http.post<any>(
          `${environment.apiUrl}/auth/firebase-login`, { idToken, role }
        ).pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }))
      )
    );
  }

  // ── TWO-FACTOR AUTH ────────────────────────────────────────
  verifyTwoFactor(userId: string, code: string) {
    return this.http.post<AuthResponse>(
      `${environment.apiUrl}/auth/2fa/verify`, { userId, code }
    ).pipe(tap(r => this.store(r)));
  }

  setup2fa()               { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/setup`,   {}); }
  enable2fa(code: string)  { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/enable`,  { code }); }
  disable2fa(code: string) { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/disable`, { code }); }
  get2faStatus()           { return this.http.get<{ enabled: boolean }>(`${environment.apiUrl}/auth/2fa/status`); }

  // ── PASSWORD RESET ─────────────────────────────────────────
  forgotPassword(email: string) {
    return this.http.post(`${environment.apiUrl}/auth/forgot-password`, { email });
  }

  // ── SESSION ────────────────────────────────────────────────
  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this._user.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }

  // ✅ Reads role from signal first, falls back to localStorage
  // Prevents race condition where signal hasn't updated yet when guard runs
  redirectAfterLogin() {
    let role = this.userRole();

    if (!role) {
      try {
        const stored = localStorage.getItem(this.USER_KEY);
        role = stored ? JSON.parse(stored)?.role ?? null : null;
      } catch {
        role = null;
      }
    }

    console.log('🟢 redirectAfterLogin — role:', role);

    if (!role) {
      console.error('❌ No role found — sending to login');
      this.router.navigate(['/auth/login']);
      return;
    }

    const map: Record<string, string> = {
      Patient: '/patient/my-appointments',
      Doctor:  '/doctor/dashboard',
      Admin:   '/admin/dashboard'
    };

    const route = map[role];
    if (!route) {
      console.error('❌ Unknown role:', role);
      this.router.navigate(['/auth/login']);
      return;
    }

    console.log('🟢 Navigating to:', route);
    this.router.navigate([route]);
  }

  // ── Internal ───────────────────────────────────────────────
  store(r: AuthResponse) {
    localStorage.setItem(this.TOKEN_KEY, r.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(r));
    this._user.set(r);
  }

  private loadUser(): AuthResponse | null {
    try { return JSON.parse(localStorage.getItem(this.USER_KEY) ?? 'null'); }
    catch { return null; }
  }
}