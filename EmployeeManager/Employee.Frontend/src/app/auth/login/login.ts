import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee-service';

import { ToastService }    from '../../services/toast.service';
import { ToastComponent }  from '../../components/toast/toast.component';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ToastComponent],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginObj   = { email: '', contactNo: '' };
  isLoading  = signal(false);

  empService = inject(EmployeeService);
  session    = inject(SessionService);
  toast      = inject(ToastService);
  router     = inject(Router);

  onLogin() {
    if (!this.loginObj.email || !this.loginObj.contactNo) {
      this.toast.warning('Please enter email and password.');
      return;
    }
    this.isLoading.set(true);
    this.empService.login(this.loginObj).subscribe({
      next: (res: any) => {
        this.session.setUser(res.data);
        this.isLoading.set(false);
        if (res.data.role === 'Employee') {
          this.router.navigateByUrl('/my-profile');
        } else {
          this.router.navigateByUrl('/dashboard');
        }
      },
      error: () => {
        this.toast.error('Invalid email or password.');
        this.isLoading.set(false);
      }
    });
  }
}