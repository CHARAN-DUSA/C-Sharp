import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { DesignationModel } from '../../shared/models/Department.models';
import { EmployeeModel } from '../../shared/models/Employee.model';
import { EmployeeService } from '../../shared/services/employee-service';
import { Master } from '../../shared/services/master';
import { SessionService } from '../../shared/services/session.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-profile.html',
  styleUrl: './my-profile.css',
})
export class MyProfile implements OnInit
{
  empService = inject(EmployeeService);
  master = inject(Master);
  session = inject(SessionService);
  toast = inject(ToastService);

  employee = signal<EmployeeModel>(new EmployeeModel());
  designationList$ !: Observable<DesignationModel[]>;
  isLoading = signal(true);
  isEditing = signal(false);
  isSaving = signal(false);

  ngOnInit()
  {
    this.designationList$ = this.master.getAllDesignation();
    const id = this.session.getUserId();
    if (id)
    {
      this.empService.getById(id).subscribe({
        next: (res: any) => { this.employee.set(res.data ?? res); this.isLoading.set(false); },
        error: () => { this.toast.error('Could not load profile.'); this.isLoading.set(false); }
      });
    }
  }

  onUpdate()
  {
    this.isSaving.set(true);
    this.empService.update(this.employee()).subscribe({
      next: () =>
      {
        this.toast.success('Profile updated successfully!');
        this.isEditing.set(false);
        this.isSaving.set(false);
      },
      error: () =>
      {
        this.toast.error('Failed to update profile.');
        this.isSaving.set(false);
      }
    });
  }

  getInitials(): string
  {
    const name = this.employee().name;
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}