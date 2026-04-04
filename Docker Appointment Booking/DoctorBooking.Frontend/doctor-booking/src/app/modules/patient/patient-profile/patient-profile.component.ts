import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';
@Component({ selector: 'app-patient-profile', standalone: true, imports: [CommonModule, ReactiveFormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './patient-profile.component.html', styleUrls: ['./patient-profile.component.css'] })
export class PatientProfileComponent implements OnInit {
  saving = signal(false); saved = signal(false);
  bloodGroups = ['A+','A-','B+','B-','AB+','AB-','O+','O-'];
  form = this.fb.group({ gender:[''],bloodGroup:[''],allergies:[''],medicalHistory:[''],emergencyContact:[''],phoneNumber:[''],address:[''] });
  constructor(private fb: FormBuilder, private http: HttpClient, readonly auth: AuthService) {}
  ngOnInit() { this.http.get<any>(`${environment.apiUrl}/patients/my-profile`).subscribe({ next: p => this.form.patchValue(p), error: () => {} }); }
  save() {
    this.saving.set(true); this.saved.set(false);
    this.http.put(`${environment.apiUrl}/patients/my-profile`, this.form.value).subscribe({
      next: () => { this.saving.set(false); this.saved.set(true); setTimeout(() => this.saved.set(false), 3000); },
      error: () => this.saving.set(false)
    });
  }
}