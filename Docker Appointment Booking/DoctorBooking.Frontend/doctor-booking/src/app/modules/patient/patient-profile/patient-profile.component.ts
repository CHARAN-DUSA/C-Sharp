import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './patient-profile.component.html',
  styleUrls: ['./patient-profile.component.css']
})
export class PatientProfileComponent implements OnInit {
  saving = signal(false);
  saved = signal(false);
  readonly navigator = navigator;

  // ── 2FA state ──────────────────────────────────────────────
  twoFaEnabled    = signal(false);
  twoFaSetupMode  = signal(false);       // show QR + verify input
  twoFaQrUrl      = signal('');
  twoFaSecret     = signal('');
  twoFaCode       = signal('');
  twoFaLoading    = signal(false);
  twoFaMessage    = signal('');
  twoFaError      = signal('');
  showDisableInput = signal(false);
  disableCode     = signal('');

  bloodGroups = ['A+','A-','B+','B-','AB+','AB-','O+','O-'];

  form = this.fb.group({
    gender:[''], bloodGroup:[''], allergies:[''],
    medicalHistory:[''], emergencyContact:[''],
    phoneNumber:[''], address:['']
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    readonly auth: AuthService
  ) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/patients/my-profile`)
      .subscribe({ next: p => this.form.patchValue(p), error: () => {} });

    // Check current 2FA status
    this.http.get<any>(`${environment.apiUrl}/auth/2fa/status`)
      .subscribe({ next: r => this.twoFaEnabled.set(r.enabled), error: () => {} });
  }

  save() {
    this.saving.set(true); this.saved.set(false);
    this.http.put(`${environment.apiUrl}/patients/my-profile`, this.form.value).subscribe({
      next: () => { this.saving.set(false); this.saved.set(true); setTimeout(() => this.saved.set(false), 3000); },
      error: () => this.saving.set(false)
    });
  }

  // ── 2FA: Begin setup ───────────────────────────────────────
  beginSetup2FA() {
    this.twoFaLoading.set(true);
    this.twoFaMessage.set(''); this.twoFaError.set('');
    this.http.post<any>(`${environment.apiUrl}/auth/2fa/setup`, {}).subscribe({
      next: r => {
        this.twoFaQrUrl.set(r.qrCodeImageUrl);
        this.twoFaSecret.set(r.manualEntryKey);
        this.twoFaSetupMode.set(true);
        this.twoFaLoading.set(false);
      },
      error: () => { this.twoFaError.set('Failed to start 2FA setup.'); this.twoFaLoading.set(false); }
    });
  }

  // ── 2FA: Verify & enable ───────────────────────────────────
  enable2FA() {
    if (!this.twoFaCode()) { this.twoFaError.set('Enter the 6-digit code.'); return; }
    this.twoFaLoading.set(true); this.twoFaError.set('');
    this.http.post<any>(`${environment.apiUrl}/auth/2fa/enable`, { code: this.twoFaCode() }).subscribe({
      next: () => {
        this.twoFaEnabled.set(true); this.twoFaSetupMode.set(false);
        this.twoFaMessage.set('Two-factor authentication enabled!');
        this.twoFaCode.set(''); this.twoFaLoading.set(false);
      },
      error: e => { this.twoFaError.set(e.error?.detail ?? 'Invalid code.'); this.twoFaLoading.set(false); }
    });
  }

  // ── 2FA: Disable ──────────────────────────────────────────
  disable2FA() {
    if (!this.disableCode()) { this.twoFaError.set('Enter your authenticator code to disable.'); return; }
    this.twoFaLoading.set(true); this.twoFaError.set('');
    this.http.post<any>(`${environment.apiUrl}/auth/2fa/disable`, { code: this.disableCode() }).subscribe({
      next: () => {
        this.twoFaEnabled.set(false); this.showDisableInput.set(false);
        this.twoFaMessage.set('Two-factor authentication disabled.');
        this.disableCode.set(''); this.twoFaLoading.set(false);
      },
      error: e => { this.twoFaError.set(e.error?.detail ?? 'Invalid code.'); this.twoFaLoading.set(false); }
    });
  }

  cancelSetup() { this.twoFaSetupMode.set(false); this.twoFaCode.set(''); this.twoFaError.set(''); }
}