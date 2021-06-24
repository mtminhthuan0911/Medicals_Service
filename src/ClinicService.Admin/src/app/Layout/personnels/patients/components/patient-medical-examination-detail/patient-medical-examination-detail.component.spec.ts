import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientMedicalExaminationDetailComponent } from './patient-medical-examination-detail.component';

describe('PatientMedicalExaminationDetailComponent', () => {
  let component: PatientMedicalExaminationDetailComponent;
  let fixture: ComponentFixture<PatientMedicalExaminationDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PatientMedicalExaminationDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PatientMedicalExaminationDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
