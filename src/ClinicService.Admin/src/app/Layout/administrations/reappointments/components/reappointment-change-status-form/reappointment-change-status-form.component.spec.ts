import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReappointmentChangeStatusFormComponent } from './reappointment-change-status-form.component';

describe('ReappointmentChangeStatusFormComponent', () => {
  let component: ReappointmentChangeStatusFormComponent;
  let fixture: ComponentFixture<ReappointmentChangeStatusFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReappointmentChangeStatusFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReappointmentChangeStatusFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
