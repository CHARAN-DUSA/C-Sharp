import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';
@Component({ selector: 'app-doctor-profile', standalone: true, imports: [CommonModule, ReactiveFormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './doctor-profile.component.html', styleUrls: ['./doctor-profile.component.css'] })
export class DoctorProfileComponent implements OnInit {
  saving = signal(false); saved = signal(false);
  form = this.fb.group({ bio:[''], consultationFee:[500], languages:['English'], clinicName:[''], address:[''] });
  constructor(private fb: FormBuilder, private http: HttpClient, readonly auth: AuthService) {}
  ngOnInit() { this.http.get<any>(`${environment.apiUrl}/doctors/my-profile`).subscribe({ next: d => this.form.patchValue(d), error: () => {} }); }
  save() {
    this.saving.set(true); this.saved.set(false);
    this.http.put(`${environment.apiUrl}/doctors/my-profile`, this.form.value).subscribe({
      next: () => { this.saving.set(false); this.saved.set(true); setTimeout(() => this.saved.set(false), 3000); },
      error: () => this.saving.set(false)
    });
  }
}