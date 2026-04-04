import { Injectable, signal } from '@angular/core';
import { initializeApp } from 'firebase/app';
import
  {
    getAuth, sendPasswordResetEmail, signInWithPopup,
    GoogleAuthProvider, RecaptchaVerifier, signInWithPhoneNumber,
    ConfirmationResult, Auth
  } from 'firebase/auth';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FirebaseService
{
  private app = initializeApp(environment.firebaseConfig);
  private auth: Auth = getAuth(this.app);
  // ✅ Use undefined consistently — matches resetOtpState()
private confirmationResult: ConfirmationResult | undefined = undefined;
private recaptchaVerifier:  RecaptchaVerifier  | undefined = undefined;

  // ── Send email password reset (Firebase) ───────────────────────────────────
  async sendEmailReset(email: string): Promise<void>
  {
    await sendPasswordResetEmail(this.auth, email);
  }

  // ── Google Sign-In (Firebase popup) ───────────────────────────────────────
  async signInWithGoogle(): Promise<{ idToken: string; email: string; name: string }>
  {
    const provider = new GoogleAuthProvider();
    const result = await signInWithPopup(this.auth, provider);
    const idToken = await result.user.getIdToken();
    return {
      idToken,
      email: result.user.email ?? '',
      name: result.user.displayName ?? ''
    };
  }

  // ── Setup reCAPTCHA for phone OTP ──────────────────────────────────────────
  setupRecaptcha(containerId: string): void
  {
    if (this.recaptchaVerifier) return;
    this.recaptchaVerifier = new RecaptchaVerifier(this.auth, containerId, {
      size: 'invisible',
      callback: () => { }
    });
  }

  // ── Send phone OTP via Firebase ────────────────────────────────────────────
  async sendPhoneOtp(phoneNumber: string): Promise<void> {
  try {
    const auth = getAuth();

    this.confirmationResult = undefined; // ✅ consistent

    if (this.recaptchaVerifier) {
      this.recaptchaVerifier.clear();
      this.recaptchaVerifier = undefined; // ✅ consistent
    }

    this.recaptchaVerifier = new RecaptchaVerifier(auth, 'recaptcha-container', {
      size: 'invisible'
    });

    await this.recaptchaVerifier.render();
    console.log('📱 Sending OTP to:', phoneNumber);

    this.confirmationResult = await signInWithPhoneNumber(
      auth,
      phoneNumber,
      this.recaptchaVerifier
    );

    console.log('✅ OTP sent successfully');

  } catch (error: any) {
    console.error('❌ Firebase sendPhoneOtp error:', error);
    console.error('Error code:', error.code);
    console.error('Error message:', error.message);
    throw error;
  }
}

  // ── Verify phone OTP ───────────────────────────────────────────────────────
  async verifyPhoneOtp(otp: string): Promise<any> {
  if (!this.confirmationResult) { // ✅ undefined check works the same as null
    throw { code: 'auth/no-confirmation', message: 'No OTP request found. Please resend.' };
  }

  try {
    const result = await this.confirmationResult.confirm(otp);
    return result;
  } catch (error: any) {
    console.error('❌ Firebase verifyPhoneOtp error:', error);
    console.error('Error code:', error.code);
    throw error;
  }
}

  // ── Reset OTP state ────────────────────────────────────────────────────────
  resetOtpState(): void
  {
    this.confirmationResult = undefined;
    this.recaptchaVerifier = undefined;
  }
}