import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
@Component({ selector: 'app-sidebar', standalone: true, imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html', styleUrls: ['./sidebar.component.css'] })
export class SidebarComponent {
  readonly isPatient = this.auth.isPatient;
  readonly isDoctor  = this.auth.isDoctor;
  readonly isAdmin   = this.auth.isAdmin;
  constructor(private auth: AuthService) {}
}