import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AsyncPipe, CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { DesignationModel } from '../../shared/models/Department.models';
import { EmployeeModel } from '../../shared/models/Employee.model';
import { EmployeeService } from '../../shared/services/employee-service';
import { Master } from '../../shared/services/master';
import { ToastService } from '../../shared/services/toast.service';


@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [FormsModule, CommonModule, AsyncPipe],
  templateUrl: './employee-form.html',   // ✅ was wrongly set to employee-list.html
  styleUrl: './employee-form.css',
})
export class EmployeeForm implements OnInit
{   // ✅ must be exported
  designationList$!: Observable<DesignationModel[]>;
  newEmployeeObj = new EmployeeModel();
  isLoading = signal(false);
  isSaving = signal(false);

  empService = inject(EmployeeService);
  master = inject(Master);
  toast = inject(ToastService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  ngOnInit()
  {
    this.designationList$ = this.master.getAllDesignation();
    this.route.paramMap.subscribe(params =>
    {
      const id = Number(params.get('id'));
      if (!isNaN(id) && id > 0)
      {
        this.isLoading.set(true);
        this.loadEmployee(id);
      } else
      {
        this.newEmployeeObj = new EmployeeModel();
      }
    });
  }

  loadEmployee(id: number)
  {
    this.empService.getById(id).subscribe({
      next: (res: any) =>
      {
        this.newEmployeeObj = res.data ?? res;
        this.isLoading.set(false);
      },
      error: () =>
      {
        this.toast.error('Failed to load employee data.');
        this.isLoading.set(false);
      }
    });
  }

  onSave()
  {
    this.isSaving.set(true);
    this.empService.create(this.newEmployeeObj).subscribe({
      next: () =>
      {
        this.toast.success('Employee saved successfully.');
        this.router.navigate(['/employees']);
      },
      error: (err: any) =>
      {
        this.toast.error('Failed to save: ' + (err.error?.title ?? 'Unknown error'));
        this.isSaving.set(false);
      }
    });
  }

  onUpdate()
  {
    this.isSaving.set(true);
    this.empService.update(this.newEmployeeObj).subscribe({
      next: () =>
      {
        this.toast.success('Employee updated successfully.');
        this.router.navigate(['/employees']);
      },
      error: (err: any) =>
      {
        this.toast.error('Failed to update: ' + (err.error?.title ?? 'Unknown error'));
        this.isSaving.set(false);
      }
    });
  }

  resetForm()
  {
    this.newEmployeeObj = new EmployeeModel();
  }
}