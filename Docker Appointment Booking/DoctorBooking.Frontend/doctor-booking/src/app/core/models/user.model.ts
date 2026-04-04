export type UserRole = 'Patient' | 'Doctor' | 'Admin';
export interface AuthResponse {
  token: string; refreshToken: string; expiration: string;
  userId: string; email: string; fullName: string; role: UserRole; profilePicture?: string;
}
export interface LoginDto { email: string; password: string; }
export interface RegisterDto {
  firstName: string; lastName: string; email: string; password: string; role: UserRole;
  phoneNumber?: string; gender?: string; dateOfBirth?: string;
  specialty?: string; qualifications?: string; consultationFee?: number;
}