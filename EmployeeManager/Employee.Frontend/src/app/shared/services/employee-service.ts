import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EmployeeModel, IEmployeeListModel, SalaryModel, TaskModel, AnnouncementModel } from '../models/Employee.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EmployeeService
{
  private http = inject(HttpClient);
  private api = environment.apiUrl;

  // ── Employee ─────────────────────────────────────────
  getAll(): Observable<IEmployeeListModel[]>
  {
    return this.http.get<IEmployeeListModel[]>(`${this.api}/EmployeeMaster`);
  }
  getById(id: number): Observable<any>
  {
    return this.http.get<any>(`${this.api}/EmployeeMaster/${id}`);
  }
  create(obj: EmployeeModel): Observable<any>
  {
    return this.http.post(`${this.api}/EmployeeMaster`, obj);
  }
  update(obj: EmployeeModel): Observable<any>
  {
    return this.http.put(`${this.api}/EmployeeMaster/${obj.employeeId}`, obj);
  }
  delete(id: number): Observable<any>
  {
    return this.http.delete(`${this.api}/EmployeeMaster/${id}`);
  }
  login(credentials: { email: string; contactNo: string }): Observable<any>
  {
    return this.http.post(`${this.api}/EmployeeMaster/login`, credentials);
  }

  // ── Salary ───────────────────────────────────────────
  getAllSalaries(): Observable<SalaryModel[]>
  {
    return this.http.get<SalaryModel[]>(`${this.api}/Salary`);
  }
  getSalaryByEmployee(empId: number): Observable<SalaryModel[]>
  {
    return this.http.get<SalaryModel[]>(`${this.api}/Salary/employee/${empId}`);
  }
  createSalary(obj: SalaryModel): Observable<any>
  {
    return this.http.post(`${this.api}/Salary`, obj);
  }
  updateSalary(id: number, obj: SalaryModel): Observable<any>
  {
    return this.http.put(`${this.api}/Salary/${id}`, obj);
  }
  deleteSalary(id: number): Observable<any>
  {
    return this.http.delete(`${this.api}/Salary/${id}`);
  }

  // ── Tasks ────────────────────────────────────────────
  getAllTasks(): Observable<TaskModel[]>
  {
    return this.http.get<TaskModel[]>(`${this.api}/Task`);
  }
  getTasksByEmployee(empId: number): Observable<TaskModel[]>
  {
    return this.http.get<TaskModel[]>(`${this.api}/Task/employee/${empId}`);
  }
  createTask(obj: TaskModel): Observable<any>
  {
    return this.http.post(`${this.api}/Task`, obj);
  }
  updateTask(id: number, obj: TaskModel): Observable<any>
  {
    return this.http.put(`${this.api}/Task/${id}`, obj);
  }
  updateTaskStatus(id: number, status: string, note: string): Observable<any>
  {
    return this.http.patch(`${this.api}/Task/${id}/status`, { status, completionNote: note });
  }
  deleteTask(id: number): Observable<any>
  {
    return this.http.delete(`${this.api}/Task/${id}`);
  }

  // ── Announcements ────────────────────────────────────
  getAnnouncements(): Observable<AnnouncementModel[]>
  {
    return this.http.get<AnnouncementModel[]>(`${this.api}/Announcement`);
  }
  createAnnouncement(obj: AnnouncementModel): Observable<any>
  {
    return this.http.post(`${this.api}/Announcement`, obj);
  }
  updateAnnouncement(id: number, obj: AnnouncementModel): Observable<any>
  {
    return this.http.put(`${this.api}/Announcement/${id}`, obj);
  }
  deleteAnnouncement(id: number): Observable<any>
  {
    return this.http.delete(`${this.api}/Announcement/${id}`);
  }


  // ── Leave ────────────────────────────────────────────
  getAllLeaves(): Observable<any[]>
  {
    return this.http.get<any[]>(`${this.api}/Leave`);
  }
  getPendingLeaves(): Observable<any[]>
  {
    return this.http.get<any[]>(`${this.api}/Leave/pending`);
  }
  getLeavesByEmployee(empId: number): Observable<any[]>
  {
    return this.http.get<any[]>(`${this.api}/Leave/employee/${empId}`);
  }
  applyLeave(obj: any): Observable<any>
  {
    return this.http.post(`${this.api}/Leave`, obj);
  }
  approveLeave(id: number, actionByEmployeeId: number): Observable<any>
  {
    return this.http.patch(`${this.api}/Leave/${id}/approve`,
      { actionByEmployeeId });
  }
  rejectLeave(id: number, actionByEmployeeId: number, reason: string): Observable<any>
  {
    return this.http.patch(`${this.api}/Leave/${id}/reject`,
      { actionByEmployeeId, reason });
  }
  cancelLeave(id: number): Observable<any>
  {
    return this.http.delete(`${this.api}/Leave/${id}`);
  }

  // ── Attendance ───────────────────────────────────────
  getAllAttendance(month?: number, year?: number): Observable<any[]>
  {
    return this.http.get<any[]>(
      `${this.api}/Attendance?month=${month ?? ''}&year=${year ?? ''}`);
  }
  getAttendanceByEmployee(empId: number, month?: number, year?: number): Observable<any[]>
  {
    return this.http.get<any[]>(
      `${this.api}/Attendance/employee/${empId}?month=${month ?? ''}&year=${year ?? ''}`);
  }
  getAttendanceSummary(empId: number, month?: number, year?: number): Observable<any>
  {
    return this.http.get<any>(
      `${this.api}/Attendance/summary/${empId}?month=${month ?? ''}&year=${year ?? ''}`);
  }
  getTodayAttendance(empId: number): Observable<any>
  {
    return this.http.get<any>(`${this.api}/Attendance/today/${empId}`);
  }
  checkIn(empId: number, notes?: string): Observable<any>
  {
    return this.http.post(`${this.api}/Attendance/checkin`,
      { employeeId: empId, notes });
  }
  checkOut(empId: number): Observable<any>
  {
    return this.http.patch(`${this.api}/Attendance/checkout`,
      { employeeId: empId });
  }
  addManualAttendance(obj: any): Observable<any>
  {
    return this.http.post(`${this.api}/Attendance/manual`, obj);
  }

  // ── Salary slip (employee self) ──────────────────────
  getMySalarySlips(empId: number): Observable<any[]>
  {
    return this.http.get<any[]>(`${this.api}/Salary/employee/${empId}`);
  }

  // ── Announcements (employee view) ───────────────────
  getMyAnnouncements(): Observable<any[]>
  {
    return this.http.get<any[]>(`${this.api}/Announcement`);
  }
}