import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Appointment } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';
@Component({ selector: 'app-doctor-dashboard', standalone: true, imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './doctor-dashboard.component.html', styleUrls: ['./doctor-dashboard.component.css'] })
export class DoctorDashboardComponent implements OnInit {
  stats = signal<any>(null); today = signal<Appointment[]>([]); loading = signal(true); completing = signal<number|null>(null);
  constructor(private http: HttpClient, readonly auth: AuthService) {}
  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/doctors/stats`).subscribe(s => this.stats.set(s));
    this.http.get<Appointment[]>(`${environment.apiUrl}/appointments/my`).subscribe({
      next: a => { const t = new Date().toDateString(); this.today.set(a.filter(x => new Date(x.appointmentDate).toDateString() === t)); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
  complete(appt: Appointment) {
    const prescription = prompt('Prescription (optional):') ?? ''; const notes = prompt('Notes (optional):') ?? '';
    this.completing.set(appt.id);
    this.http.patch(`${environment.apiUrl}/appointments/${appt.id}/complete`, { rowVersion: appt.rowVersion, prescription, notes }).subscribe({
      next: () => { this.completing.set(null); this.ngOnInit(); }, error: (e:any) => { this.completing.set(null); alert(e.error?.detail ?? 'Failed'); }
    });
  }
}