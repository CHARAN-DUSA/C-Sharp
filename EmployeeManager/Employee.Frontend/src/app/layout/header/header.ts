import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SessionService } from '../../services/session.service';
import { ToastComponent } from '../../components/toast/toast.component';
import { EmployeeModel } from '../../models/Employee.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterOutlet, NgIf, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header
{
  isCollapsed = false;
  session = inject(SessionService);
  router = inject(Router);

  get user(): EmployeeModel
  {
    return this.session.getUser() ?? new EmployeeModel();
  }

  get isHR(): boolean { return this.session.isHR(); }

  toggleSidebar() { this.isCollapsed = !this.isCollapsed; }

  getInitials(): string
  {
    const name = this.user.name;
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  logout()
  {
    this.session.clear();
    this.router.navigate(['/login']);
  }
}