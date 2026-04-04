import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { interval, startWith, switchMap } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';
@Component({ selector: 'app-navbar', standalone: true, imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html', styleUrls: ['./navbar.component.css'] })
export class NavbarComponent implements OnInit {
  unread = signal(0);
  readonly isLoggedIn  = this.auth.isLoggedIn;
  readonly name        = computed(() => this.auth.currentUser()?.fullName ?? '');
  readonly role        = computed(() => this.auth.currentUser()?.role ?? '');
  readonly roleLower   = computed(() => (this.auth.currentUser()?.role ?? '').toLowerCase());
  readonly initials    = computed(() => { const n = this.auth.currentUser()?.fullName ?? ''; return n.split(' ').map((w:string) => w[0]).join('').toUpperCase().slice(0,2); });
  readonly profileLink = computed(() => { const r = this.auth.userRole(); return r === 'Doctor' ? '/doctor/profile' : r === 'Admin' ? '/admin/dashboard' : '/patient/profile'; });
  constructor(private auth: AuthService, private http: HttpClient) {}
  ngOnInit() {
    if (this.auth.isLoggedIn()) {
      interval(10000).pipe(startWith(0), switchMap(() => this.http.get<number>(`${environment.apiUrl}/notifications/unread-count`)))
        .subscribe({ next: n => this.unread.set(n), error: () => {} });
    }
  }
  logout() { this.auth.logout(); }
}