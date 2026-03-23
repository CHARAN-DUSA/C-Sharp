import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../shared/services/employee-service';
import { SessionService } from '../../shared/services/session.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './leaves.html',
  styleUrl: './leaves.css'
})
export class Leaves implements OnInit {
  empService = inject(EmployeeService);
  session    = inject(SessionService);
  toast      = inject(ToastService);

  leaveList    = signal<any[]>([]);
  isLoading    = signal(true);
  isSaving     = signal(false);
  rejectModal  = signal<any | null>(null);
  rejectionReason = '';

  leaveObj = {
    employeeId: 0,
    leaveType: 'Casual',
    fromDate: '',
    toDate: '',
    reason: ''
  };

  get isHR()  { return this.session.isHR(); }
  get userId() { return this.session.getUserId(); }

  get pendingCount()  { return this.leaveList().filter(l => l.status === 'Pending').length; }
  get approvedCount() { return this.leaveList().filter(l => l.status === 'Approved').length; }
  get rejectedCount() { return this.leaveList().filter(l => l.status === 'Rejected').length; }

  ngOnInit() { this.loadLeaves(); }

  loadLeaves() {
    this.isLoading.set(true);
    const obs = this.isHR
      ? this.empService.getAllLeaves()
      : this.empService.getLeavesByEmployee(this.userId);

    obs.subscribe({
      next: res => { this.leaveList.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onApply() {
    this.isSaving.set(true);
    this.leaveObj.employeeId = this.userId;
    this.empService.applyLeave(this.leaveObj).subscribe({
      next: () => {
        this.toast.success('Leave applied successfully.');
        this.resetForm();
        this.loadLeaves();
      },
      error: (err) => {
        this.toast.error(err.error ?? 'Error applying leave.');
        this.isSaving.set(false);
      }
    });
  }

  onApprove(id: number) {
    this.empService.approveLeave(id, this.userId).subscribe({
      next: () => { this.toast.success('Leave approved.'); this.loadLeaves(); },
      error: () => this.toast.error('Error approving leave.')
    });
  }

  openRejectModal(leave: any) {
    this.rejectModal.set(leave);
    this.rejectionReason = '';
  }

  confirmReject() {
    const leave = this.rejectModal();
    if (!leave) return;
    this.empService.rejectLeave(leave.leaveId, this.userId, this.rejectionReason).subscribe({
      next: () => {
        this.toast.success('Leave rejected.');
        this.rejectModal.set(null);
        this.loadLeaves();
      },
      error: () => this.toast.error('Error rejecting leave.')
    });
  }

  onCancel(id: number) {
    if (!confirm('Cancel this leave request?')) return;
    this.empService.cancelLeave(id).subscribe({
      next: () => { this.toast.success('Leave cancelled.'); this.loadLeaves(); },
      error: (err) => this.toast.error(err.error ?? 'Error cancelling.')
    });
  }

  statusClass(s: string) {
    return { 'Approved': 'bg-success', 'Rejected': 'bg-danger', 'Pending': 'bg-warning text-dark' }[s] ?? 'bg-secondary';
  }

  resetForm() {
    this.leaveObj = { employeeId: 0, leaveType: 'Casual', fromDate: '', toDate: '', reason: '' };
    this.isSaving.set(false);
  }
}