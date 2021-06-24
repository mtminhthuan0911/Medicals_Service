import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalExaminationChangeStatusFormComponent } from './medical-examination-change-status-form.component';

describe('MedicalExaminationChangeStatusFormComponent', () => {
  let component: MedicalExaminationChangeStatusFormComponent;
  let fixture: ComponentFixture<MedicalExaminationChangeStatusFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalExaminationChangeStatusFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalExaminationChangeStatusFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
