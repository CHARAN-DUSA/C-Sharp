import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnnouncementModel } from '../../shared/models/Employee.model';
import { EmployeeService } from '../../shared/services/employee-service';
import { ToastService } from '../../shared/services/toast.service';


@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './announcements.html',
  styleUrl: './announcements.css'
})
export class Announcements implements OnInit {
  empService = inject(EmployeeService);
  toast      = inject(ToastService);

  list      = signal<AnnouncementModel[]>([]);
  isLoading = signal(true);
  isSaving  = signal(false);
  isEditing = signal(false);
  obj       = new AnnouncementModel();

  ngOnInit() { this.load(); }

  load() {
    this.isLoading.set(true);
    this.empService.getAnnouncements().subscribe({
      next: res => { this.list.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onEdit(a: AnnouncementModel) {
    this.obj = { ...a };
    this.isEditing.set(true);
  }

  onSave() {
    this.isSaving.set(true);
    this.empService.createAnnouncement(this.obj).subscribe({
      next: () => { this.toast.success('Announcement posted.'); this.reset(); this.load(); },
      error: () => { this.toast.error('Error posting announcement.'); this.isSaving.set(false); }
    });
  }

  onUpdate() {
    this.isSaving.set(true);
    this.empService.updateAnnouncement(this.obj.announcementId, this.obj).subscribe({
      next: () => { this.toast.success('Updated.'); this.reset(); this.load(); },
      error: () => { this.toast.error('Error updating.'); this.isSaving.set(false); }
    });
  }

  onDelete(id: number) {
    if (!confirm('Delete this announcement?')) return;
    this.empService.deleteAnnouncement(id).subscribe({
      next: () => { this.toast.success('Deleted.'); this.load(); },
      error: () => this.toast.error('Error deleting.')
    });
  }

  targetBadge(role: string) {
    return { 'All': 'bg-primary', 'HR': 'bg-success', 'Employee': 'bg-info text-dark' }[role] ?? 'bg-secondary';
  }

  reset() { this.obj = new AnnouncementModel(); this.isEditing.set(false); this.isSaving.set(false); }
}