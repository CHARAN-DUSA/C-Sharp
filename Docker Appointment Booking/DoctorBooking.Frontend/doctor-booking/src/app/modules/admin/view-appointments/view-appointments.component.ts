import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Appointment } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-view-appointments', standalone: true, imports: [CommonModule, FormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './view-appointments.component.html', styleUrls: ['./view-appointments.component.css'] })
export class ViewAppointmentsComponent implements OnInit {
  appointments = signal<Appointment[]>([]); total = signal(0); loading = signal(true); page = signal(1); pageSize = 20; statusFilter = ''; dateFilter = '';
  get totalPages() { return Math.ceil(this.total() / this.pageSize); }
  constructor(private http: HttpClient) {}
  ngOnInit() { this.load(); }
  load() {
    this.loading.set(true);
    const params: any = { pageNumber: this.page(), pageSize: this.pageSize };
    if (this.statusFilter) params['status'] = this.statusFilter;
    if (this.dateFilter) params['date'] = this.dateFilter;
    this.http.get<any>(`${environment.apiUrl}/admin/appointments`, { params }).subscribe({
      next: r => { this.appointments.set(r.items); this.total.set(r.totalCount); this.loading.set(false); }, error: () => this.loading.set(false)
    });
  }
  changePage(p: number) { this.page.set(p); this.load(); }
}