import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../services/employee-service';
import { ToastService }    from '../../services/toast.service';
import { SalaryModel, IEmployeeListModel } from '../../models/Employee.model';

@Component({
  selector: 'app-salary',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './salary.html',
  styleUrl: './salary.css'
})
export class Salary implements OnInit {
  empService = inject(EmployeeService);
  toast      = inject(ToastService);

  salaryList   = signal<SalaryModel[]>([]);
  employeeList = signal<IEmployeeListModel[]>([]);
  isLoading    = signal(true);
  isSaving     = signal(false);
  isEditing    = signal(false);

  salaryObj = new SalaryModel();

  months = [
    { value: 1, label: 'January' }, { value: 2, label: 'February' },
    { value: 3, label: 'March' },   { value: 4, label: 'April' },
    { value: 5, label: 'May' },     { value: 6, label: 'June' },
    { value: 7, label: 'July' },    { value: 8, label: 'August' },
    { value: 9, label: 'September' },{ value: 10, label: 'October' },
    { value: 11, label: 'November' },{ value: 12, label: 'December' }
  ];

  get years() {
    const y = new Date().getFullYear();
    return [y - 2, y - 1, y, y + 1];
  }

  get netSalaryPreview(): number {
    return (this.salaryObj.basicSalary || 0) +
           (this.salaryObj.hra || 0) +
           (this.salaryObj.da || 0) +
           (this.salaryObj.bonus || 0) -
           (this.salaryObj.deductions || 0);
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading.set(true);
    this.empService.getAll().subscribe({ next: res => this.employeeList.set(res) });
    this.empService.getAllSalaries().subscribe({
      next: res => { this.salaryList.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getMonthName(m: number) {
    return this.months.find(x => x.value === m)?.label ?? m;
  }

  onEdit(s: SalaryModel) {
    this.salaryObj = { ...s };
    this.isEditing.set(true);
  }

  onSave() {
    this.isSaving.set(true);
    this.empService.createSalary(this.salaryObj).subscribe({
      next: () => { this.toast.success('Salary saved.'); this.reset(); this.loadData(); },
      error: (err) => { this.toast.error(err.error ?? 'Error saving salary.'); this.isSaving.set(false); }
    });
  }

  onUpdate() {
    this.isSaving.set(true);
    this.empService.updateSalary(this.salaryObj.salaryId, this.salaryObj).subscribe({
      next: () => { this.toast.success('Salary updated.'); this.reset(); this.loadData(); },
      error: () => { this.toast.error('Error updating salary.'); this.isSaving.set(false); }
    });
  }

  onDelete(id: number) {
    if (!confirm('Delete this salary record?')) return;
    this.empService.deleteSalary(id).subscribe({
      next: () => { this.toast.success('Deleted.'); this.loadData(); },
      error: () => this.toast.error('Error deleting.')
    });
  }

  reset() {
    this.salaryObj = new SalaryModel();
    this.isEditing.set(false);
    this.isSaving.set(false);
  }
}