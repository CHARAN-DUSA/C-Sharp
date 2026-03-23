import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../services/employee-service';
import { SessionService }  from '../../services/session.service';
import { ToastService }    from '../../services/toast.service';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './attendance.html',
  styleUrl: './attendance.css'
})
export class Attendance implements OnInit {
  empService = inject(EmployeeService);
  session    = inject(SessionService);
  toast      = inject(ToastService);

  attendanceList = signal<any[]>([]);
  todayRecord    = signal<any | null>(null);
  summary        = signal<any | null>(null);
  isLoading      = signal(true);
  isActing       = signal(false);
  now: Date = new Date();

  selectedMonth = signal(new Date().getMonth() + 1);
  selectedYear  = signal(new Date().getFullYear());

  // HR manual entry
  manualObj = {
    employeeId: 0,
    date: new Date().toISOString().split('T')[0],
    checkIn: '',
    checkOut: '',
    status: 'Present',
    notes: ''
  };

  employeeList = signal<any[]>([]);

  get isHR()   { return this.session.isHR(); }
  get userId() { return this.session.getUserId(); }

  get months() {
    return [
      { v: 1, l: 'January' }, { v: 2, l: 'February' }, { v: 3, l: 'March' },
      { v: 4, l: 'April' },   { v: 5, l: 'May' },      { v: 6, l: 'June' },
      { v: 7, l: 'July' },    { v: 8, l: 'August' },   { v: 9, l: 'September' },
      { v: 10, l: 'October' },{ v: 11, l: 'November' },{ v: 12, l: 'December' }
    ];
  }

  get years() {
    const y = new Date().getFullYear();
    return [y - 1, y, y + 1];
  }

  get canCheckIn() {
    return !this.todayRecord() && !this.isActing();
  }

  get canCheckOut() {
    return this.todayRecord() && !this.todayRecord().checkOut && !this.isActing();
  }

  ngOnInit() {
    if (this.isHR) {
      this.empService.getAll().subscribe({ next: res => this.employeeList.set(res) });
    }
    this.loadData();
    if (!this.isHR) {
      this.loadToday();
      this.loadSummary();
    }
  }

  loadData() {
    this.isLoading.set(true);
    const m = this.selectedMonth();
    const y = this.selectedYear();

    const obs = this.isHR
      ? this.empService.getAllAttendance(m, y)
      : this.empService.getAttendanceByEmployee(this.userId, m, y);

    obs.subscribe({
      next: res => { this.attendanceList.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  loadToday() {
    this.empService.getTodayAttendance(this.userId).subscribe({
      next: res => this.todayRecord.set(res)
    });
  }

  loadSummary() {
    this.empService.getAttendanceSummary(
      this.userId, this.selectedMonth(), this.selectedYear()
    ).subscribe({
      next: res => this.summary.set(res)
    });
  }

  onCheckIn() {
    this.isActing.set(true);
    this.empService.checkIn(this.userId).subscribe({
      next: () => {
        this.toast.success('Checked in successfully!');
        this.loadToday();
        this.loadData();
        this.isActing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error ?? 'Error checking in.');
        this.isActing.set(false);
      }
    });
  }

  onCheckOut() {
    this.isActing.set(true);
    this.empService.checkOut(this.userId).subscribe({
      next: () => {
        this.toast.success('Checked out successfully!');
        this.loadToday();
        this.loadData();
        this.isActing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error ?? 'Error checking out.');
        this.isActing.set(false);
      }
    });
  }

  onAddManual() {
    this.empService.addManualAttendance(this.manualObj).subscribe({
      next: () => {
        this.toast.success('Attendance added.');
        this.loadData();
        this.manualObj = {
          employeeId: 0, date: new Date().toISOString().split('T')[0],
          checkIn: '', checkOut: '', status: 'Present', notes: ''
        };
      },
      error: (err) => this.toast.error(err.error ?? 'Error adding attendance.')
    });
  }

  statusClass(s: string) {
    return {
      'Present': 'bg-success', 'Absent': 'bg-danger',
      'HalfDay': 'bg-warning text-dark', 'Leave': 'bg-info text-dark'
    }[s] ?? 'bg-secondary';
  }

  formatTime(t: any) {
    if (!t) return '—';
    const parts = t.split(':');
    const h = parseInt(parts[0]);
    const m = parts[1];
    return `${h % 12 || 12}:${m} ${h < 12 ? 'AM' : 'PM'}`;
  }
}