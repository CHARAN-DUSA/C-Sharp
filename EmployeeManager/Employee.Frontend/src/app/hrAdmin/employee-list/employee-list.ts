import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { IEmployeeListModel } from '../../shared/models/Employee.model';
import { EmployeeService } from '../../shared/services/employee-service';
import { ToastService } from '../../shared/services/toast.service';


@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css',
})
export class EmployeeList implements OnInit
{
  employeeList = signal<IEmployeeListModel[]>([]);
  isLoading = signal(true);
  deletingId = signal<number | null>(null);

  empService = inject(EmployeeService);
  toast = inject(ToastService);

  ngOnInit() { this.loadAll(); }

  loadAll()
  {
    this.isLoading.set(true);
    this.empService.getAll().subscribe({
      next: (res) => { this.employeeList.set(res); this.isLoading.set(false); },
      error: () => { this.toast.error('Failed to load employees.'); this.isLoading.set(false); }
    });
  }

  onDelete(item: IEmployeeListModel)
  {
    if (!confirm(`Delete ${item.name}?`)) return;
    this.deletingId.set(item.employeeId);
    this.empService.delete(item.employeeId).subscribe({
      next: () =>
      {
        this.toast.success(`${item.name} deleted.`);
        this.deletingId.set(null);
        this.loadAll();
      },
      error: () =>
      {
        this.toast.error('Failed to delete employee.');
        this.deletingId.set(null);
      }
    });
  }
}