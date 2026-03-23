import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyAnnouncements } from './my-announcements';

describe('MyAnnouncements', () => {
  let component: MyAnnouncements;
  let fixture: ComponentFixture<MyAnnouncements>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyAnnouncements]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyAnnouncements);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
