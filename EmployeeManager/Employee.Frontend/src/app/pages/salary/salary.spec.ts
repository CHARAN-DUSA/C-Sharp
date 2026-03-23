import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Salary } from './salary';

describe('Salary', () => {
  let component: Salary;
  let fixture: ComponentFixture<Salary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Salary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Salary);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
