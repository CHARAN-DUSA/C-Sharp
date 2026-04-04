import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({
  selector: 'app-admin-dashboard', standalone: true, 
  imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './admin-dashboard.component.html', 
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit
{
  stats = signal<any>(null); loading = signal(true);
  constructor(private http: HttpClient) { }
  ngOnInit() { this.http.get<any>(`${environment.apiUrl}/admin/stats`).subscribe({ next: s => { this.stats.set(s); this.loading.set(false); }, error: () => this.loading.set(false) }); }
}