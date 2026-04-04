import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  loading = signal(false);
  error   = signal('');

  form = this.fb.group({
    firstName:      ['', [Validators.required, Validators.maxLength(50)]],
    lastName:       ['', [Validators.required, Validators.maxLength(50)]],
    email:          ['', [Validators.required, Validators.email]],
    phoneNumber:    ['', [Validators.required, Validators.pattern(/^\+[1-9]\d{7,14}$/)]],
    password:       ['', [Validators.required, Validators.minLength(6),
                          Validators.pattern(/(?=.*[A-Z])(?=.*[0-9])/)]],
    role:           ['Patient', Validators.required],
    gender:         [''],
    // Doctor-specific
    specialty:      [''],
    qualifications: [''],
    consultationFee:[500]
  });

  get isDoctor() { return this.form.get('role')?.value === 'Doctor'; }

  constructor(private fb: FormBuilder, private auth: AuthService) {}

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set('');
    this.auth.register(this.form.value as any).subscribe({
      next:  () => { this.loading.set(false); this.auth.redirectAfterLogin(); },
      error: (e: any) => {
        this.loading.set(false);
        this.error.set(e.error?.detail ?? 'Registration failed. Please try again.');
      }
    });
  }
}