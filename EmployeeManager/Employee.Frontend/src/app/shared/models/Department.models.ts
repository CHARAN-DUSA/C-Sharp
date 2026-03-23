export class DepartmentModel {
  departmentId: number;
  departmentName: string;
  isActive: boolean;


constructor() {
    this.departmentId = 0;
    this.departmentName = '';
    this.isActive = false;
    }
}

export interface DesignationModel {
  designationId: number;
  departmentId: number;
  designationName: string;
  }