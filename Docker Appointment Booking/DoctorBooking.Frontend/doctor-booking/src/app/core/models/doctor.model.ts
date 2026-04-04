export interface Doctor {
  id: number; userId: string; firstName: string; lastName: string; fullName: string;
  email: string; phoneNumber?: string; specialty: string; subSpecialty?: string;
  qualifications: string; experienceYears: number; bio: string; consultationFee: number;
  rating: number; totalReviews: number; profilePicture?: string; isAvailable: boolean;
  isVerified: boolean; languages: string; workingDays: string; clinicName?: string; address?: string;
}
export interface Review { id: number; patientId: number; doctorId: number; patientName: string; appointmentId: number; rating: number; comment: string; createdAt: string; }