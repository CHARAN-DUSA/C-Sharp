import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';

interface TimeSlot {
  id: number;
  date: string;
  startTime: string;
  endTime: string;
  isBooked: boolean;
  isBlocked: boolean;
}

@Component({
  selector: 'app-manage-availability',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './manage-availability.component.html',
  styleUrls: ['./manage-availability.component.css']
})
export class ManageAvailabilityComponent implements OnInit {
  loading      = signal(false);
  slotsLoading = signal(false);
  error        = signal('');
  success      = signal('');
  slots        = signal<TimeSlot[]>([]);
  editingSlot  = signal<TimeSlot | null>(null);
  editStart    = '';
  editEnd      = '';
  viewDate     = '';

  form = this.fb.group({
    date:                [this.todayStr(), Validators.required],
    startTime:           ['09:00', Validators.required],
    endTime:             ['17:00', Validators.required],
    slotDurationMinutes: [30, Validators.required]
  });

  constructor(private fb: FormBuilder, private http: HttpClient) {}

  ngOnInit() {
    // Load today's slots on init
    this.viewDate = this.todayStr();
    this.loadSlots(this.viewDate);
  }

  todayStr() {
    return new Date().toISOString().split('T')[0];
  }

  get minDate() { return this.todayStr(); }

  // Generate slots for the selected date
  generate() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    this.http.post(`${environment.apiUrl}/doctors/availability`, this.form.value).subscribe({
      next: (r: any) => {
        this.loading.set(false);
        this.success.set(r.message ?? 'Slots generated successfully!');
        // Load slots for the selected date
        this.viewDate = this.form.value.date!;
        this.loadSlots(this.viewDate);
      },
      error: (e: any) => {
        this.loading.set(false);
        this.error.set(e.error?.detail ?? 'Failed to generate slots.');
      }
    });
  }

  // Load slots for a given date
  loadSlots(date: string) {
    this.slotsLoading.set(true);
    this.http.get<TimeSlot[]>(`${environment.apiUrl}/doctors/my-slots`, { params: { date } }).subscribe({
      next:  s => { this.slots.set(s); this.slotsLoading.set(false); },
      error: () => this.slotsLoading.set(false)
    });
  }

  // Cancel (delete) a slot
  cancelSlot(slot: TimeSlot) {
    if (slot.isBooked) return;
    if (!confirm(`Cancel slot ${slot.startTime} – ${slot.endTime}?`)) return;
    this.http.delete(`${environment.apiUrl}/doctors/slots/${slot.id}`).subscribe({
      next:  () => this.loadSlots(this.viewDate),
      error: (e: any) => this.error.set(e.error?.detail ?? 'Failed to cancel slot.')
    });
  }

  // Start editing a slot time
  startEdit(slot: TimeSlot) {
    this.editingSlot.set(slot);
    this.editStart = slot.startTime;
    this.editEnd   = slot.endTime;
  }

  saveEdit() {
    const slot = this.editingSlot();
    if (!slot) return;
    this.http.patch(`${environment.apiUrl}/doctors/slots/${slot.id}`, {
      slotId:    slot.id,
      startTime: this.editStart,
      endTime:   this.editEnd
    }).subscribe({
      next: () => {
        this.editingSlot.set(null);
        this.loadSlots(this.viewDate);
        this.success.set('Slot time updated.');
      },
      error: (e: any) => this.error.set(e.error?.detail ?? 'Failed to update slot.')
    });
  }

  cancelEdit() { this.editingSlot.set(null); }

  onViewDateChange(date: string) {
    this.viewDate = date;
    this.loadSlots(date);
  }

  slotStatusClass(slot: TimeSlot) {
    if (slot.isBooked)  return 'bg-danger text-white';
    if (slot.isBlocked) return 'bg-secondary text-white';
    return 'bg-success text-white';
  }

  slotStatusLabel(slot: TimeSlot) {
    if (slot.isBooked)  return 'Booked';
    if (slot.isBlocked) return 'Blocked';
    return 'Available';
  }
}