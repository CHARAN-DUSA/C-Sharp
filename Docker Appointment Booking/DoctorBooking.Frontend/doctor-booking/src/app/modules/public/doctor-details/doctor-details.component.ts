import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Doctor, Review } from '../../../core/models/doctor.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { AuthService } from '../../../core/services/auth.service';
@Component({ selector: 'app-doctor-details', standalone: true, imports: [CommonModule, RouterModule, NavbarComponent],
  templateUrl: './doctor-details.component.html', styleUrls: ['./doctor-details.component.css'] })
export class DoctorDetailsComponent implements OnInit {
  doctor = signal<Doctor | null>(null); reviews = signal<Review[]>([]); loading = signal(true);
  readonly isPatient = this.auth.isPatient;
  constructor(private route: ActivatedRoute, private http: HttpClient, private auth: AuthService) {}
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.http.get<Doctor>(`${environment.apiUrl}/doctors/${id}`).subscribe({ next: d => { this.doctor.set(d); this.loading.set(false); }, error: () => this.loading.set(false) });
    this.http.get<Review[]>(`${environment.apiUrl}/reviews/doctor/${id}`).subscribe(r => this.reviews.set(r));
  }
  stars(r: number) { return Array.from({length:5},(_,i) => i < Math.round(r)); }
}