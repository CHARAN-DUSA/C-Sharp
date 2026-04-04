import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Review } from '../../../core/models/doctor.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
@Component({
  selector: 'app-reviews', standalone: true, imports: [CommonModule, ReactiveFormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './reviews.component.html', styleUrls: ['./reviews.component.css']
})
export class ReviewsComponent implements OnInit
{
  reviews = signal<Review[]>([]); saving = signal(false); saved = signal(false);
  stars = [1, 2, 3, 4, 5];
  form = this.fb.group({ doctorId: [1, Validators.required], appointmentId: [1, Validators.required], rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]], comment: ['', [Validators.required, Validators.minLength(10)]] });
  constructor(private fb: FormBuilder, private http: HttpClient) { }
  ngOnInit() { this.http.get<Review[]>(`${environment.apiUrl}/reviews/my`).subscribe(r => this.reviews.set(r)); }
  setRating(v: number) { this.form.get('rating')?.setValue(v); }
  getRating(): number { return this.form.get('rating')?.value ?? 0; }
  submit()
  {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true); this.saved.set(false);
    this.http.post(`${environment.apiUrl}/reviews`, this.form.value).subscribe({
      next: () => { this.saving.set(false); this.saved.set(true); this.form.reset({ rating: 5 }); this.ngOnInit(); },
      error: () => this.saving.set(false)
    });
  }
  starsFill(r: number) { return Array.from({ length: 5 }, (_, i) => i < Math.round(r)); }
}