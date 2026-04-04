import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-manage-doctors', standalone: true, imports: [CommonModule, NavbarComponent, SidebarComponent],
  templateUrl: './manage-doctors.component.html', styleUrls: ['./manage-doctors.component.css'] })
export class ManageDoctorsComponent implements OnInit {
  doctors = signal<any[]>([]); loading = signal(true);
  constructor(private http: HttpClient) {}
  ngOnInit() { this.load(); }
  load() { this.http.get<any>(`${environment.apiUrl}/admin/doctors`).subscribe({ next: r => { this.doctors.set(r.items); this.loading.set(false); }, error: () => this.loading.set(false) }); }
  verify(id: number) { this.http.patch(`${environment.apiUrl}/admin/doctors/${id}/verify`, {}).subscribe(() => this.load()); }
}