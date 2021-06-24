import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReappointmentFormComponent } from './reappointment-form.component';

describe('ReappointmentFormComponent', () => {
  let component: ReappointmentFormComponent;
  let fixture: ComponentFixture<ReappointmentFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReappointmentFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReappointmentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
