import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast-item toast-{{ toast.type }}" (click)="toastService.remove(toast.id)">
          <i class="fas {{ iconFor(toast.type) }}"></i>
          <span>{{ toast.message }}</span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed; bottom: 24px; right: 24px;
      display: flex; flex-direction: column; gap: 10px;
      z-index: 9999; pointer-events: none;
    }
    .toast-item {
      display: flex; align-items: center; gap: 10px;
      padding: 12px 18px; border-radius: 10px;
      font-size: 14px; font-weight: 500;
      box-shadow: 0 4px 16px rgba(0,0,0,0.12);
      cursor: pointer; pointer-events: all;
      animation: slideIn 0.3s ease;
      max-width: 340px;
    }
    @keyframes slideIn {
      from { transform: translateX(100%); opacity: 0; }
      to   { transform: translateX(0);   opacity: 1; }
    }
    .toast-success { background: #f0fdf4; color: #15803d; border-left: 4px solid #16a34a; }
    .toast-error   { background: #fef2f2; color: #b91c1c; border-left: 4px solid #dc2626; }
    .toast-warning { background: #fffbeb; color: #b45309; border-left: 4px solid #d97706; }
    .toast-info    { background: #eff6ff; color: #1d4ed8; border-left: 4px solid #2563eb; }
  `]
})
export class ToastComponent {
  toastService = inject(ToastService);

  iconFor(type: string): string {
    return { success: 'fa-check-circle', error: 'fa-times-circle',
             warning: 'fa-exclamation-triangle', info: 'fa-info-circle' }[type] ?? 'fa-info-circle';
  }
}