import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './doctor-profile.component.html',
  styleUrls: ['./doctor-profile.component.css']
})
export class DoctorProfileComponent implements OnInit {
  saving = signal(false);
  saved  = signal(false);
  readonly navigator = navigator;

  // ── 2FA state ──────────────────────────────────────────────
  twoFaEnabled     = signal(false);
  twoFaSetupMode   = signal(false);
  twoFaQrUrl       = signal('');
  twoFaSecret      = signal('');
  twoFaCode        = signal('');
  twoFaLoading     = signal(false);
  twoFaMessage     = signal('');
  twoFaError       = signal('');
  showDisableInput = signal(false);
  disableCode      = signal('');

  form = this.fb.group({
    bio: [''], consultationFee: [500],
    languages: ['English'], clinicName: [''], address: ['']
  });

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    readonly auth: AuthService
  ) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/doctors/my-profile`)
      .subscribe({ next: d => this.form.patchValue(d), error: () => {} });

    this.http.get<any>(`${environment.apiUrl}/auth/2fa/status`)
      .subscribe({ next: r => this.twoFaEnabled.set(r.enabled), error: () => {} });
  }

  save() {
    this.saving.set(true); this.saved.set(false);
    this.http.put(`${environment.apiUrl}/doctors/my-profile`, this.form.value).subscribe({
      next: () => { this.saving.set(false); this.saved.set(true); setTimeout(() => this.saved.set(false), 3000); },
      error: () => this.saving.set(false)
    });
  }

  beginSetup2FA() {
    this.twoFaLoading.set(true); this.twoFaMessage.set(''); this.twoFaError.set('');
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