import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Notification } from '../../../core/models/chat.model';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({
  selector: 'app-notifications', standalone: true, imports: [CommonModule, NavbarComponent, SidebarComponent, TimeAgoPipe],
  templateUrl: './notifications.component.html', styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit
{
  notifications = signal<Notification[]>([]); loading = signal(true);
  get unreadCount() { return this.notifications().filter(n => !n.isRead).length; }
  constructor(private http: HttpClient) { }
  ngOnInit() { this.http.get<Notification[]>(`${environment.apiUrl}/notifications`).subscribe({ next: n => { this.notifications.set(n); this.loading.set(false); }, error: () => this.loading.set(false) }); }
  markRead(n: Notification) { if (n.isRead) return; this.http.patch(`${environment.apiUrl}/notifications/${n.id}/read`, {}).subscribe(() => this.notifications.update(ns => ns.map(x => x.id === n.id ? { ...x, isRead: true } : x))); }
  markAllRead() { this.http.patch(`${environment.apiUrl}/notifications/read-all`, {}).subscribe(() => this.notifications.update(ns => ns.map(n => ({ ...n, isRead: true })))); }
}