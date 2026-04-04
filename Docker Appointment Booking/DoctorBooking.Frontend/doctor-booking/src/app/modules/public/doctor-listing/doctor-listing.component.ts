import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Doctor } from '../../../core/models/doctor.model';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-doctor-listing',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NavbarComponent, SidebarComponent],
  templateUrl: './doctor-listing.component.html',
  styleUrls: ['./doctor-listing.component.css']
})
export class DoctorListingComponent implements OnInit {
  doctors     = signal<Doctor[]>([]);
  specialties = signal<string[]>([]);
  loading     = signal(true);
  search      = '';
  specialty   = '';

  readonly isLoggedIn = this.auth.isLoggedIn;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private auth: AuthService
  ) {}

  ngOnInit() {
    // Load all doctors immediately with no filters
    this.load();

    // Then apply query params if any
    this.route.queryParams.subscribe(p => {
      this.search    = p['search']    ?? '';
      this.specialty = p['specialty'] ?? '';
      this.load();
    });

    // Load specialties for filter dropdown
    this.http.get<string[]>(`${environment.apiUrl}/doctors/specialties`)
      .subscribe({
        next:  s => this.specialties.set(s),
        error: () => {}
      });
  }

  load() {
    this.loading.set(true);
    const params: any = {};
    if (this.search)    params['search']    = this.search;
    if (this.specialty) params['specialty'] = this.specialty;

    this.http.get<Doctor[]>(`${environment.apiUrl}/doctors`, { params }).subscribe({
      next:  d => { this.doctors.set(d); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  clearFilters() {
    this.search = '';
    this.specialty = '';
    this.load();
  }

  stars(r: number) {
    return Array.from({ length: 5 }, (_, i) => i < Math.round(r));
  }
}