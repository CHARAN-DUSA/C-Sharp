import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
      
      <!-- Logo / Brand -->
      <div class="mb-8 text-center">
        <div class="w-16 h-16 bg-indigo-600 rounded-2xl flex items-center justify-center mx-auto mb-4 shadow-lg">
          <svg class="w-9 h-9 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 
                 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"/>
          </svg>
        </div>
        <h1 class="text-2xl font-bold text-gray-800">DoctorBooking</h1>
      </div>

      <!-- Spinner -->
      <div class="bg-white rounded-2xl shadow-xl px-10 py-8 flex flex-col items-center gap-4 min-w-[280px]">
        
        <!-- Google icon + spinner row -->
        <div class="relative flex items-center justify-center w-14 h-14">
          <!-- Spinning ring -->
          <svg class="animate-spin w-14 h-14 text-indigo-500" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10"
              stroke="currentColor" stroke-width="3"/>
            <path class="opacity-75" fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/>
          </svg>
          <!-- Google G in center -->
          <svg class="absolute w-6 h-6" viewBox="0 0 24 24">
            <path fill="#4285F4"
              d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26
                 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74
                 3.28-8.09z"/>
            <path fill="#34A853"
              d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23
                 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99
                 20.53 7.7 23 12 23z"/>
            <path fill="#FBBC05"
              d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43
                 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
            <path fill="#EA4335"
              d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09
                 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6
                 3.3-4.53 6.16-4.53z"/>
          </svg>
        </div>

        <div class="text-center">
          <p class="text-gray-800 font-semibold text-lg">Signing you in</p>
          <p class="text-gray-400 text-sm mt-1">Please wait a moment...</p>
        </div>

        <!-- Animated dots -->
        <div class="flex gap-1.5 mt-1">
          <span class="w-2 h-2 bg-indigo-400 rounded-full animate-bounce [animation-delay:0ms]"></span>
          <span class="w-2 h-2 bg-indigo-400 rounded-full animate-bounce [animation-delay:150ms]"></span>
          <span class="w-2 h-2 bg-indigo-400 rounded-full animate-bounce [animation-delay:300ms]"></span>
        </div>
      </div>

      <p class="text-gray-400 text-xs mt-6">Secured by Google OAuth 2.0</p>
    </div>
  `
})
export class CallbackComponent implements OnInit {
  constructor(
    private auth:   AuthService,
    private router: Router
  ) {}

  ngOnInit() {
  const hash    = window.location.hash.substring(1);
  const params  = new URLSearchParams(hash);
  const idToken = params.get('id_token');
  const state   = params.get('state');
  const saved   = localStorage.getItem('oauth_state');

  if (!idToken || state !== saved) {
    this.router.navigate(['/login']);
    return;
  }

  localStorage.removeItem('oauth_state');

  // ✅ Read return URL if present
  const returnUrl = localStorage.getItem('oauth_return_url') ?? null;
  localStorage.removeItem('oauth_return_url');

  this.auth.googleLogin(idToken, 'Patient').subscribe({
    next: () => {
      if (returnUrl) {
        this.router.navigate([returnUrl]); // ✅ go back to forgot-password
      } else {
        this.auth.redirectAfterLogin();    // ✅ normal login flow
      }
    },
    error: () => this.router.navigate(['/login'],
                   { queryParams: { error: 'google_failed' } })
  });
}
}