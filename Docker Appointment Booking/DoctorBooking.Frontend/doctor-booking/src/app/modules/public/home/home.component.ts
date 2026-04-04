import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
@Component({ selector: 'app-home', standalone: true, imports: [CommonModule, RouterModule, FormsModule, NavbarComponent],
  templateUrl: './home.component.html', styleUrls: ['./home.component.css'] })
export class HomeComponent {
  searchQuery = ''; selectedSpecialty = '';
  heroStats = [{value:'500+',label:'Doctors'},{value:'50K+',label:'Patients'},{value:'4.9★',label:'Avg Rating'},{value:'24/7',label:'Support'}];
  specialties = [
    {name:'Cardiology',icon:'bi-heart-pulse',bg:'#fee2e2',color:'#dc2626',count:24},
    {name:'Neurology',icon:'bi-cpu',bg:'#ede9fe',color:'#7c3aed',count:18},
    {name:'Orthopedics',icon:'bi-person-walking',bg:'#dbeafe',color:'#2563eb',count:31},
    {name:'Dermatology',icon:'bi-droplet-half',bg:'#fce7f3',color:'#be185d',count:22},
    {name:'Pediatrics',icon:'bi-emoji-smile',bg:'#dcfce7',color:'#16a34a',count:28},
    {name:'Gynecology',icon:'bi-gender-female',bg:'#ffedd5',color:'#ea580c',count:15},
    {name:'Psychiatry',icon:'bi-brain',bg:'#e0f2fe',color:'#0284c7',count:12},
    {name:'General',icon:'bi-hospital',bg:'#eff6ff',color:'#1d4ed8',count:47}
  ];
  howItWorks = [
    {title:'Search & Select',desc:'Browse verified doctors by specialty or name. Read reviews and check live availability.'},
    {title:'Book Your Slot',desc:'Pick a time slot. RowVersion concurrency prevents double-booking in real time.'},
    {title:'Get Confirmation',desc:'Instant email confirmation. Doctor notified. Show up and get care.'}
  ];
  features = [
    {icon:'bi-shield-lock',title:'JWT Secured',desc:'Role-based access for Patient, Doctor, and Admin.'},
    {icon:'bi-lightning-charge',title:'No Double Booking',desc:'RowVersion concurrency prevents two patients booking the same slot.'},
    {icon:'bi-chat-dots',title:'Chat with Doctors',desc:'Real-time messaging. Auto-polls every 3s — no refresh needed.'},
    {icon:'bi-bell',title:'Smart Notifications',desc:'Email reminders, confirmations, and live in-app alerts.'}
  ];
  constructor(private router: Router) {}
  search() { this.router.navigate(['/doctors'], { queryParams: { search: this.searchQuery, specialty: this.selectedSpecialty } }); }
  goSpecialty(name: string) { this.router.navigate(['/doctors'], { queryParams: { specialty: name } }); }
}