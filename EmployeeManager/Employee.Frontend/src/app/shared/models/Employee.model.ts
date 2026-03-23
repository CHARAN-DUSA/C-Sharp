export class EmployeeModel {
  employeeId: number = 0;
  name: string = '';
  contactNo: string = '';
  email: string = '';
  city: string = '';
  state: string = '';
  pincode: string = '';
  address: string = '';
  designationName: string = '';
  altContactNo?: string = '';
  role?: string = '';
  designationId: number = 0;
  createdDate?: Date;
  modifiedDate?: Date;
}

export interface IEmployeeListModel {
  employeeId: number;
  name: string;
  contactNo: string;
  email: string;
  city: string;
  state: string;
  pincode: string;
  altContactNo?: string;
  address: string;
  designationId: number;
  designationName: string;
  departmentId: number;
  departmentName: string;
  role?: string;
  createdDate?: Date;
  modifiedDate?: Date;
}

// ── NEW ──────────────────────────────────────────
export class SalaryModel {
  salaryId: number = 0;
  employeeId: number = 0;
  employeeName?: string = '';
  basicSalary: number = 0;
  hra: number = 0;
  da: number = 0;
  bonus: number = 0;
  deductions: number = 0;
  netSalary: number = 0;
  month: number = new Date().getMonth() + 1;
  year: number = new Date().getFullYear();
  remarks?: string = '';
  createdDate?: Date;
}

export class TaskModel {
  taskId: number = 0;
  title: string = '';
  description?: string = '';
  assignedToEmployeeId: number = 0;
  assignedToName?: string = '';
  assignedByEmployeeId?: number;
  dueDate: string = '';
  status: string = 'Pending';
  priority: string = 'Medium';
  createdDate?: Date;
  completedDate?: Date;
  completionNote?: string = '';
}

export class AnnouncementModel {
  announcementId: number = 0;
  title: string = '';
  content: string = '';
  targetRole: string = 'All';
  createdDate?: Date;
  isActive: boolean = true;
}