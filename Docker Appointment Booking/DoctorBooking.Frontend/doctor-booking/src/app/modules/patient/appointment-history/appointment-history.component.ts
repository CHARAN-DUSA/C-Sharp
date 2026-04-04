import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Appointment } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-appointment-history', standalone: true, imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './appointment-history.component.html', styleUrls: ['./appointment-history.component.css'] })
export class AppointmentHistoryComponent implements OnInit {
  appointments = signal<Appointment[]>([]); loading = signal(true);
  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.http.get<Appointment[]>(`${environment.apiUrl}/appointments/my?status=Completed`).subscribe({
      next: a => { this.appointments.set(a); this.loading.set(false); }, error: () => this.loading.set(false)
    });
  }
}