import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({ selector: 'app-manage-users', standalone: true, imports: [CommonModule, NavbarComponent, SidebarComponent],
  templateUrl: './manage-users.component.html', styleUrls: ['./manage-users.component.css'] })
export class ManageUsersComponent implements OnInit {
  users = signal<any[]>([]); total = signal(0); loading = signal(true);
  constructor(private http: HttpClient) {}
  ngOnInit() { this.load(); }
  load() { this.http.get<any>(`${environment.apiUrl}/admin/users`).subscribe({ next: r => { this.users.set(r.items); this.total.set(r.totalCount); this.loading.set(false); }, error: () => this.loading.set(false) }); }
  toggleStatus(u: any) { this.http.patch(`${environment.apiUrl}/admin/users/${u.id}/toggle-status`, {}).subscribe(() => this.load()); }
}