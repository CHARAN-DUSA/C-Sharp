export type AppointmentStatus = 'Pending' | 'Confirmed' | 'Completed' | 'Cancelled' | 'NoShow';
export interface Appointment {
  id: number; patientId: number; doctorId: number; patientName: string; doctorName: string;
  doctorSpecialty: string; appointmentDate: string; timeSlot: string;
  status: AppointmentStatus; statusText: string; reason: string;
  notes?: string; prescription?: string; consultationFee: number; rowVersion: string; createdAt: string;
}
export interface BookAppointmentDto { doctorId: number; appointmentDate: string; timeSlot: string; reason: string; }
export interface TimeSlot { id: number; doctorId: number; date: string; startTime: string; endTime: string; isBooked: boolean; isBlocked: boolean; }