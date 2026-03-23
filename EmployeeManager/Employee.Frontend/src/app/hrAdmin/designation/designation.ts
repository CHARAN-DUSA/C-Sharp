import { Component, inject, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Master } from '../../shared/services/master';
import { DesignationModel } from '../../shared/models/Department.models';

@Component({
  selector: 'app-designation-reactive',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, AsyncPipe],
  templateUrl: './designation.html'
})
export class Designation implements OnInit
{

  masterService = inject(Master);
  fb = inject(FormBuilder);

  designationForm!: FormGroup;

  deptList = signal<any[]>([]);
  designationList$!: Observable<DesignationModel[]>;

  isEditMode = signal(false);   // 🔥 signal version
  isLoading = signal(false);

  ngOnInit(): void
  {
    this.createForm();
    this.getDepartments();
    this.getDesignations();
  }

  createForm()
  {
    this.designationForm = this.fb.group({
      designationId: [0],
      departmentId: [0, Validators.required],
      designationName: ['', Validators.required]
    });
  }

  // 🔥 GET DEPARTMENTS
  getDepartments()
  {
    this.isLoading.set(true);

    this.masterService.getAllDept().subscribe({
      next: (res: any[]) =>
      {
        this.deptList.set(res);
        this.isLoading.set(false);
      },
      error: (err) =>
      {
        alert(err.error);
        this.isLoading.set(false);
      }
    });
  }

  // 🔥 GET DESIGNATIONS
  getDesignations()
  {
    this.isLoading.set(true);

    this.designationList$ = this.masterService.getAllDesignation();

    // small delay simulation (optional)
    setTimeout(() =>
    {
      this.isLoading.set(false);
    }, 500);
  }

  // 🔥 SAVE
  onSaveDesignation()
  {
    if (this.designationForm.invalid) return;

    this.isLoading.set(true);

    this.masterService.saveDesignation(this.designationForm.value).subscribe({
      next: () =>
      {
        alert('Saved');
        this.getDesignations();
        this.resetForm();
        this.isLoading.set(false);
      },
      error: err =>
      {
        alert(err.error);
        this.isLoading.set(false);
      }
    });
  }

  // 🔥 EDIT
  onEditDesignation(data: DesignationModel)
  {
    this.designationForm.patchValue(data);
    this.isEditMode.set(true);
  }

  // 🔥 UPDATE
  onUpdateDesignation()
  {
    this.isLoading.set(true);

    this.masterService.updateDesignation(this.designationForm.value).subscribe({
      next: () =>
      {
        alert('Updated');
        this.getDesignations();
        this.resetForm();
        this.isEditMode.set(false);
        this.isLoading.set(false);
      },
      error: err =>
      {
        alert(err.error);
        this.isLoading.set(false);
      }
    });
  }

  // 🔥 DELETE
  onDeleteDesignation(data: DesignationModel)
  {
    if (!confirm('Delete this designation?')) return;

    this.isLoading.set(true);

    this.masterService.deleteDesignation(data.designationId).subscribe({
      next: () =>
      {
        this.getDesignations();
        this.isLoading.set(false);
      },
      error: err =>
      {
        alert(err.error);
        this.isLoading.set(false);
      }
    });
  }

  // 🔥 RESET
  resetForm()
  {
    this.designationForm.reset({
      designationId: 0,
      departmentId: 0,
      designationName: ''
    });

    this.isEditMode.set(false);
  }

  // 🔥 HELPER
  getDeptName(id: number)
  {
    const dept = this.deptList().find(d => d.departmentId == id);
    return dept ? dept.departmentName : '';
  }
}