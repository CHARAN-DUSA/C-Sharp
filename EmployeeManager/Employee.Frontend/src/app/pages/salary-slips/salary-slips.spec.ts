import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalarySlips } from './salary-slips';

describe('SalarySlips', () => {
  let component: SalarySlips;
  let fixture: ComponentFixture<SalarySlips>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalarySlips]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SalarySlips);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
