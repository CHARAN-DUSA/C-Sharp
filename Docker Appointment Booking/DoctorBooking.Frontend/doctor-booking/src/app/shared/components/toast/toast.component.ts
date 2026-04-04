import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
export interface ToastItem { id: number; message: string; type: 'success'|'danger'|'warning'|'info'; }
@Component({ selector: 'app-toast', standalone: true, imports: [CommonModule],
  template: `<div class="position-fixed bottom-0 end-0 p-3" style="z-index:9999">
    <div *ngFor="let t of toasts()" class="toast show align-items-center border-0 mb-2" [class]="'text-bg-' + t.type" style="border-radius:10px;min-width:280px">
      <div class="d-flex"><div class="toast-body fw-semibold">{{ t.message }}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" (click)="remove(t.id)"></button></div>
    </div></div>`,
  styleUrls: ['./toast.component.css'] })
export class ToastComponent {
  toasts = signal<ToastItem[]>([]); private nextId = 0;
  show(message: string, type: ToastItem['type'] = 'success', duration = 3500) {
    const id = ++this.nextId; this.toasts.update(ts => [...ts, { id, message, type }]);
    setTimeout(() => this.remove(id), duration);
  }
  remove(id: number) { this.toasts.update(ts => ts.filter(t => t.id !== id)); }
}