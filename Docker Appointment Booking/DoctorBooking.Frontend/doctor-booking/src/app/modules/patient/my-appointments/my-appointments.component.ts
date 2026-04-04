import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Appointment } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-my-appointments', standalone: true, imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './my-appointments.component.html', styleUrls: ['./my-appointments.component.css'] })
export class MyAppointmentsComponent implements OnInit {
  appointments = signal<Appointment[]>([]); loading = signal(true); cancelling = signal<number|null>(null); error = signal('');
  constructor(private http: HttpClient) {}
  ngOnInit() { this.load(); }
  load() {
    this.loading.set(true);
    this.http.get<Appointment[]>(`${environment.apiUrl}/appointments/my?status=Confirmed`).subscribe({
      next: a => { this.appointments.set(a); this.loading.set(false); }, error: () => this.loading.set(false)
    });
  }
  cancel(appt: Appointment) {
    if (!confirm('Cancel this appointment?')) return;
    this.cancelling.set(appt.id); this.error.set('');
    this.http.patch(`${environment.apiUrl}/appointments/${appt.id}/cancel`, { rowVersion: appt.rowVersion }).subscribe({
      next: () => { this.cancelling.set(null); this.load(); },
      error: (e:any) => { this.cancelling.set(null); this.error.set(e.error?.detail ?? 'Cancellation failed — concurrent modification detected.'); }
    });
  }
  statusClass(s: string): Record<string,boolean> { return {'status-upcoming':s==='Confirmed','status-pending':s==='Pending','status-cancelled':s==='Cancelled','status-completed':s==='Completed'}; }
}