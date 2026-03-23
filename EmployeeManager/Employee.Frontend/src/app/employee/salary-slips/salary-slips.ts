import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeService } from '../../shared/services/employee-service';
import { SessionService } from '../../shared/services/session.service';


@Component({
  selector: 'app-salary-slip',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './salary-slips.html',
  styleUrl: './salary-slips.css'
})
export class SalarySlip implements OnInit {
  empService  = inject(EmployeeService);
  session     = inject(SessionService);

  slips        = signal<any[]>([]);
  selectedSlip = signal<any | null>(null);
  isLoading    = signal(true);

  months = ['','January','February','March','April','May','June',
            'July','August','September','October','November','December'];

  ngOnInit() {
    const id = this.session.getUserId();
    this.empService.getMySalarySlips(id).subscribe({
      next: res => { this.slips.set(res); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  viewSlip(slip: any) { this.selectedSlip.set(slip); }
  closeSlip()         { this.selectedSlip.set(null); }

  getMonthName(m: number) { return this.months[m] ?? m; }

  printSlip() { window.print(); }
}