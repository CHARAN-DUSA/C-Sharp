import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Appointment } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-doctor-appointments', standalone: true, imports: [CommonModule, FormsModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './doctor-appointments.component.html', styleUrls: ['./doctor-appointments.component.css'] })
export class DoctorAppointmentsComponent implements OnInit {
  appointments = signal<Appointment[]>([]); loading = signal(true); activeFilter = signal('');
  filters = [{label:'All',value:''},{label:'Pending',value:'Pending'},{label:'Confirmed',value:'Confirmed'},{label:'Completed',value:'Completed'},{label:'Cancelled',value:'Cancelled'}];
  constructor(private http: HttpClient) {}
  ngOnInit() { this.load(); }
  filterBy(v: string) { this.activeFilter.set(v); this.load(); }
  load() {
    this.loading.set(true);
    const params: any = {}; if (this.activeFilter()) params['status'] = this.activeFilter();
    this.http.get<Appointment[]>(`${environment.apiUrl}/appointments/my`, { params }).subscribe({
      next: a => { this.appointments.set(a); this.loading.set(false); }, error: () => this.loading.set(false)
    });
  }
  complete(a: Appointment) {
    const prescription = prompt('Prescription:') ?? ''; const notes = prompt('Notes:') ?? '';
    this.http.patch(`${environment.apiUrl}/appointments/${a.id}/complete`, { rowVersion: a.rowVersion, prescription, notes }).subscribe({ next: () => this.load(), error: (e:any) => alert(e.error?.detail) });
  }
}