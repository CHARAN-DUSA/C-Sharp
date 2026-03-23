import { Component, inject, OnInit, signal } from '@angular/core';
import { DepartmentModel } from '../../models/Department.models';
import { FormsModule } from '@angular/forms';
import { Master } from '../../services/master';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-department',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './department.html',
  styleUrl: './department.css',
})
export class Department implements OnInit
{

  newDeptObj: DepartmentModel = new DepartmentModel();

  masterService = inject(Master);
  deptList = signal<DepartmentModel[]>([]);

  ngOnInit(): void
  {
    this.getAllDepartments();
  }

  onSaveDept()
  {
    if (!this.newDeptObj.departmentName)
    {
      alert('Enter Department Name');
      return;
    }
    this.masterService.saveDept(this.newDeptObj).subscribe({
      next: (res: any) =>
      {
        this.getAllDepartments();
        this.resetForm();
      },
      error: (err) =>
      {
        alert(err.error);
      }
    });
  }

  resetForm()
  {
    this.newDeptObj = new DepartmentModel();
  }

  onEdit(data: DepartmentModel)
  {
    const strData = JSON.stringify(data);
    const parsedData = JSON.parse(strData);
    this.newDeptObj = { ...parsedData };
  }


  getAllDepartments()
  {
    this.masterService.getAllDept().subscribe({
      next: (result: any) =>
      {
        this.deptList.set(result);
      }, error: (error) =>
      {
        alert(error.error);
        console.error('Error fetching departments:', error);
      }
    });
  }

  onUpdateDept()
  {
    this.masterService.updateDept(this.newDeptObj).subscribe({
      next: (res: any) =>
      {
        this.getAllDepartments();
        this.resetForm();
        alert(`Department "${res.departmentName}" updated successfully`);
      },
      error: (err) =>
      {
        alert(err.error);
      }
    });
  }

  onDelete(data: DepartmentModel)
  {
    if (confirm(`Are you sure you want to delete department "${data.departmentName}"?`))
    {
      this.masterService.deleteDept(data.departmentId).subscribe({
        next: (res: any) =>
        {
          this.getAllDepartments();
          alert(`Department "${data.departmentName}" deleted successfully`);
        },
        error: (err) =>
        {
          alert(err.error);
        }
      });
    }
  }
}
