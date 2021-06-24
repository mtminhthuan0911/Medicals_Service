import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentChangeStatusFormComponent } from './appointment-change-status-form.component';

describe('AppointmentChangeStatusFormComponent', () => {
  let component: AppointmentChangeStatusFormComponent;
  let fixture: ComponentFixture<AppointmentChangeStatusFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentChangeStatusFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentChangeStatusFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
