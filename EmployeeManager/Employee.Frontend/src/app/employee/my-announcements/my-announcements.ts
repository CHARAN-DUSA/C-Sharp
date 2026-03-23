import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../shared/services/employee-service';
import { SessionService } from '../../shared/services/session.service';


@Component({
  selector: 'app-my-announcements',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-announcements.html',
  styleUrl: './my-announcements.css'
})
export class MyAnnouncements implements OnInit {
  empService = inject(EmployeeService);
  session    = inject(SessionService);

  list      = signal<any[]>([]);
  isLoading = signal(true);

  get role() { return this.session.getRole(); }

  ngOnInit() {
    this.empService.getMyAnnouncements().subscribe({
      next: res => {
        // filter by role — show All + own role
        const filtered = res.filter(a =>
          a.targetRole === 'All' || a.targetRole === this.role);
        this.list.set(filtered);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  targetColor(role: string) {
    return { 'All': '#0ea5e9', 'HR': '#16a34a', 'Employee': '#7c3aed' }[role] ?? '#64748b';
  }
}