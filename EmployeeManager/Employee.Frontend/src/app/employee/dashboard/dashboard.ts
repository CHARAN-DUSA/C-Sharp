import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { IEmployeeListModel, AnnouncementModel } from '../../shared/models/Employee.model';
import { EmployeeService } from '../../shared/services/employee-service';
import { Master } from '../../shared/services/master';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit
{
  empService = inject(EmployeeService);
  masterService = inject(Master);

  employees = signal<IEmployeeListModel[]>([]);
  deptCount = signal(0);
  designationCount = signal(0);
  isLoading = signal(true);
  overdueTasks  = signal(0);
  announcements = signal<AnnouncementModel[]>([]);

  get empCount() { return this.employees().length; }
  get hrCount() { return this.employees().filter(e => e.role === 'HR').length; }
  get recentEmployees() { return this.employees().slice(0, 5); }

  ngOnInit()
  {
    this.empService.getAll().subscribe({          // ✅ was getAllEmployee
      next: (res: IEmployeeListModel[]) =>
      {      // ✅ explicit type fixes TS7006
        this.employees.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });

    this.masterService.getAllDept().subscribe({
      next: (res) => this.deptCount.set(res.length)
    });

    this.masterService.getAllDesignation().subscribe({
      next: (res) => this.designationCount.set(res.length)
    });

    // Add to ngOnInit() in dashboard.ts
this.empService.getAllTasks().subscribe({
  next: (res) => this.overdueTasks.set(
    res.filter(t => t.status !== 'Completed' && new Date(t.dueDate) < new Date()).length
  )
});

this.empService.getAnnouncements().subscribe({
  next: (res) => this.announcements.set(res.slice(0, 3))
});

  }
}