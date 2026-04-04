import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, from, switchMap, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginDto, RegisterDto, UserRole } from '../models/user.model';

// Firebase imports
import { initializeApp, getApps } from 'firebase/app';
import { getAuth, RecaptchaVerifier, signInWithPhoneNumber, ConfirmationResult } from 'firebase/auth';

// ✅ Initialize Firebase once
if (!getApps().length)
{
  initializeApp(environment.firebaseConfig);
}

@Injectable({ providedIn: 'root' })
export class AuthService
{
  private readonly TOKEN_KEY = 'mb_token';
  private readonly USER_KEY = 'mb_user';
  private _user = signal<AuthResponse | null>(this.loadUser());

  readonly currentUser = this._user.asReadonly();
  readonly isLoggedIn = computed(() => !!this._user());
  readonly userRole = computed((): UserRole | null => this._user()?.role ?? null);
  readonly isPatient = computed(() => this._user()?.role === 'Patient');
  readonly isDoctor = computed(() => this._user()?.role === 'Doctor');
  readonly isAdmin = computed(() => this._user()?.role === 'Admin');

  // ✅ Store Firebase confirmation result between sendOtp and verifyOtp
  private confirmationResult: ConfirmationResult | null = null;
  private recaptchaVerifier: RecaptchaVerifier | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  // ── EMAIL/PASSWORD ────────────────────────────────────
  login(dto: LoginDto)
  {
    return this.http.post<any>(`${environment.apiUrl}/auth/login`, dto)
      .pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  register(dto: RegisterDto)
  {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, dto)
      .pipe(tap(r => this.store(r)));
  }

  // ── GOOGLE OAUTH ──────────────────────────────────────
  googleLogin(idToken: string, role: string = 'Patient')
  {
    return this.http.post<any>(`${environment.apiUrl}/auth/google`, { idToken, role })
      .pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  // ✅ Add this method — keeps store() private
  loginWithPhone(phoneNumber: string, password: string)
  {
    return this.http.post<any>(`${environment.apiUrl}/auth/login-phone`, {
      phoneNumber,
      password
    }).pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }));
  }

  // ── PHONE OTP VIA FIREBASE ────────────────────────────
  sendOtp(phoneNumber: string, role: string = 'Patient'): Observable<void>
  {
    const auth = getAuth();


    // ✅ FIX #3 — Safer clear pattern
    if (this.recaptchaVerifier)
    {
      try
      {
        this.recaptchaVerifier.clear();
      } catch (e)
      {
        console.warn('Recaptcha clear error:', e);
      }
      this.recaptchaVerifier = null;
    }

    // ✅ FIX #2 — Check container exists before creating verifier
    const container = document.getElementById('recaptcha-container');
    if (!container)
    {
      return new Observable(o => o.error({ error: { detail: 'Recaptcha container not found in DOM.' } }));
    }

    this.recaptchaVerifier = new RecaptchaVerifier(auth, 'recaptcha-container', {
      size: 'invisible'
    });

    return from(
      signInWithPhoneNumber(auth, phoneNumber, this.recaptchaVerifier)
        .then(result => { this.confirmationResult = result; })
    );
  }

  verifyOtp(phoneNumber: string, otpCode: string, role: string = 'Patient'): Observable<any>
  {
    if (!this.confirmationResult)
      return new Observable(o => o.error({
        error: { detail: 'No OTP request found. Please resend.' }
      }));

    return from(
      this.confirmationResult.confirm(otpCode)
        .then(result => result.user.getIdToken())
    ).pipe(
      // ✅ Send Firebase ID token to your existing firebase-login endpoint
      switchMap(idToken =>
        this.http.post<any>(`${environment.apiUrl}/auth/firebase-login`, { idToken, role })
          .pipe(tap(r => { if (!r.requiresTwoFactor) this.store(r); }))
      )
    );
  }

  // ── TWO-FACTOR AUTH ───────────────────────────────────
  verifyTwoFactor(userId: string, code: string)
  {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/2fa/verify`, { userId, code })
      .pipe(tap(r => this.store(r)));
  }
  setup2fa() { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/setup`, {}); }
  enable2fa(code: string) { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/enable`, { code }); }
  disable2fa(code: string) { return this.http.post<any>(`${environment.apiUrl}/auth/2fa/disable`, { code }); }

  // ── PASSWORD RESET ────────────────────────────────────
  forgotPassword(email: string)
  {
    return this.http.post(`${environment.apiUrl}/auth/forgot-password`, { email });
  }

  // ── SESSION ───────────────────────────────────────────
  logout()
  {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this._user.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }

  redirectAfterLogin()
  {
    const map: Record<UserRole, string> = {
      Patient: '/patient/my-appointments',
      Doctor: '/doctor/dashboard',
      Admin: '/admin/dashboard'
    };
    this.router.navigate([map[this.userRole()!]]);
  }

  private store(r: AuthResponse)
  {
    localStorage.setItem(this.TOKEN_KEY, r.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(r));
    this._user.set(r);
  }


  private loadUser(): AuthResponse | null
  {
    try { return JSON.parse(localStorage.getItem(this.USER_KEY) ?? 'null'); }
    catch { return null; }
  }
}