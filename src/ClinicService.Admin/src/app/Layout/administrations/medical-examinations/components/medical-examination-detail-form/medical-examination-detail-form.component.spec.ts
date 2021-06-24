import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalExaminationDetailFormComponent } from './medical-examination-detail-form.component';

describe('MedicalExaminationDetailFormComponent', () => {
  let component: MedicalExaminationDetailFormComponent;
  let fixture: ComponentFixture<MedicalExaminationDetailFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalExaminationDetailFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalExaminationDetailFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
