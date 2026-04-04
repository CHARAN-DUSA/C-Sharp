import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Doctor } from '../../../core/models/doctor.model';
import { TimeSlot } from '../../../core/models/appointment.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({
  selector: 'app-book-appointment', standalone: true, imports: [CommonModule, ReactiveFormsModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './book-appointment.component.html', styleUrls: ['./book-appointment.component.css']
})
export class BookAppointmentComponent implements OnInit
{
  doctor = signal<Doctor | null>(null); slots = signal<TimeSlot[]>([]); loading = signal(false);
  slotsLoading = signal(false); success = signal(false); error = signal(''); selectedDate = ''; selectedSlot = '';
  form = this.fb.group({ date: ['', Validators.required], reason: ['', [Validators.required, Validators.maxLength(500)]] });
  constructor(private fb: FormBuilder, private route: ActivatedRoute, private http: HttpClient) { }
  ngOnInit()
  {
    const id = this.route.snapshot.paramMap.get('doctorId')!;
    this.http.get<Doctor>(`${environment.apiUrl}/doctors/${id}`).subscribe(d => this.doctor.set(d));
  }
  onDateChange(date: string)
  {
    this.selectedDate = date; this.selectedSlot = ''; this.slotsLoading.set(true);
    this.http.get<TimeSlot[]>(`${environment.apiUrl}/doctors/${this.doctor()?.id}/slots`, { params: { date } }).subscribe({
      next: s => { this.slots.set(s); this.slotsLoading.set(false); }, error: () => this.slotsLoading.set(false)
    });
  }
  selectSlot(slot: TimeSlot) { if (!slot.isBooked && !slot.isBlocked) this.selectedSlot = slot.startTime; }
  submit()
  {
    if (this.form.invalid || !this.selectedSlot) { this.error.set('Please select a date, time slot, and reason.'); return; }
    this.loading.set(true); this.error.set('');
    this.http.post(`${environment.apiUrl}/appointments`, { doctorId: this.doctor()!.id, appointmentDate: this.selectedDate, timeSlot: this.selectedSlot, reason: this.form.value.reason }).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); },
      error: (e: any) => { this.loading.set(false); this.error.set(e.error?.detail ?? 'Booking failed. Slot may have just been taken.'); }
    });
  }
  get minDate() { return new Date().toISOString().split('T')[0]; }
}