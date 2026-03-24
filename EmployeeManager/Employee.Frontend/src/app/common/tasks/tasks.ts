import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../shared/services/employee-service';
import { SessionService } from '../../shared/services/session.service';
import { ToastService } from '../../shared/services/toast.service';
import { TaskModel, IEmployeeListModel } from '../../shared/models/Employee.model';


@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.html',
  styleUrl: './tasks.css'
})
export class Tasks implements OnInit {
  empService = inject(EmployeeService);
  session    = inject(SessionService);
  toast      = inject(ToastService);

  taskList     = signal<TaskModel[]>([]);
  employeeList = signal<IEmployeeListModel[]>([]);
  isLoading    = signal(true);
  isSaving     = signal(false);
  isEditing    = signal(false);
  filterStatus = signal('All');

  taskObj = new TaskModel();

  // status update modal state
  updatingTask  = signal<TaskModel | null>(null);
  statusNote    = signal('');
  newStatus     = signal('');
  hrMessage     = signal('');

  get isHR() { return this.session.isHR(); }
  get userId() { return this.session.getUserId(); }

  filteredTasks = computed(() => {
    const tasks = this.taskList();
    const f = this.filterStatus();
    return f === 'All' ? tasks : tasks.filter(t => t.status === f);
  });

  get pendingCount()    { return this.taskList().filter(t => t.status === 'Pending').length; }
  get inProgressCount() { return this.taskList().filter(t => t.status === 'InProgress').length; }
  get completedCount()  { return this.taskList().filter(t => t.status === 'Completed').length; }

  ngOnInit() {
    this.empService.getAll().subscribe({ next: res => this.employeeList.set(res) });
    this.loadTasks();
  }

  loadTasks() {
    this.isLoading.set(true);
    const obs = this.isHR
      ? this.empService.getAllTasks()
      : this.empService.getTasksByEmployee(this.userId);

    obs.subscribe({
      next: res => { this.taskList.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onEdit(t: TaskModel) {
    this.taskObj = { ...t, dueDate: t.dueDate ? new Date(t.dueDate).toISOString().split('T')[0] : '' };
    this.isEditing.set(true);
  }

  onSave() {
    this.isSaving.set(true);
    this.taskObj.assignedByEmployeeId = this.userId;
    this.empService.createTask(this.taskObj).subscribe({
      next: () => { this.toast.success('Task created.'); this.reset(); this.loadTasks(); },
      error: () => { this.toast.error('Error creating task.'); this.isSaving.set(false); }
    });
  }

  onUpdate() {
    this.isSaving.set(true);
    this.empService.updateTask(this.taskObj.taskId, this.taskObj).subscribe({
      next: () => { this.toast.success('Task updated.'); this.reset(); this.loadTasks(); },
      error: () => { this.toast.error('Error updating task.'); this.isSaving.set(false); }
    });
  }

  onDelete(id: number) {
    if (!confirm('Delete this task?')) return;
    this.empService.deleteTask(id).subscribe({
      next: () => { this.toast.success('Task deleted.'); this.loadTasks(); },
      error: () => this.toast.error('Error deleting task.')
    });
  }

  openStatusUpdate(t: TaskModel) {
    this.updatingTask.set(t);
    this.newStatus.set(t.status);
    this.statusNote.set(t.completionNote ?? '');
    this.hrMessage.set(t.hrMessage ?? '');
  }

  confirmStatusUpdate() {
  const t = this.updatingTask();
  if (!t) return;

  this.empService.updateTaskStatus(
    t.taskId,
    {
      status: this.newStatus(),
      completionNote: this.statusNote(),
      hrMessage: this.hrMessage()
    }
  ).subscribe({
    next: () => {
      this.toast.success('Task status updated.');
      this.updatingTask.set(null);
      this.loadTasks();
    },
    error: () => this.toast.error('Error updating status.')
  });
}

  priorityClass(p: string) {
    return { 'High': 'bg-danger', 'Medium': 'bg-warning text-dark', 'Low': 'bg-success' }[p] ?? 'bg-secondary';
  }

  statusClass(s: string) {
    return { 'Completed': 'bg-success', 'InProgress': 'bg-info text-dark', 'Pending': 'bg-warning text-dark' }[s] ?? 'bg-secondary';
  }

  isOverdue(dueDate: any) {
    return new Date(dueDate) < new Date() ;
  }

  reset() {
    this.taskObj = new TaskModel();
    this.isEditing.set(false);
    this.isSaving.set(false);
  }
}