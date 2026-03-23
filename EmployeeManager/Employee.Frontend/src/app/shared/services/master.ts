import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DepartmentModel, DesignationModel } from '../models/Department.models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class Master
{
  private http = inject(HttpClient);
  private deptApi = environment.apiUrl + '/DepartmentMaster';
  private desigApi = environment.apiUrl + '/DesignationMaster';

  getAllDept(): Observable<DepartmentModel[]>
  {
    return this.http.get<DepartmentModel[]>(this.deptApi);
  }
  saveDept(obj: any) { return this.http.post(this.deptApi, obj); }
  updateDept(obj: any) { return this.http.put(`${this.deptApi}/${obj.departmentId}`, obj); }
  deleteDept(id: number) { return this.http.delete(`${this.deptApi}/${id}`); }

  getAllDesignation(): Observable<DesignationModel[]>
  {
    return this.http.get<DesignationModel[]>(this.desigApi);
  }
  saveDesignation(obj: any) { return this.http.post(this.desigApi, obj); }
  updateDesignation(obj: any) { return this.http.put(`${this.desigApi}/${obj.designationId}`, obj); }
  deleteDesignation(id: number) { return this.http.delete(`${this.desigApi}/${id}`); }
}