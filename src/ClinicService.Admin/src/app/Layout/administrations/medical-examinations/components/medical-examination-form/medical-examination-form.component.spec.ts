import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalExaminationFormComponent } from './medical-examination-form.component';

describe('MedicalExaminationFormComponent', () => {
  let component: MedicalExaminationFormComponent;
  let fixture: ComponentFixture<MedicalExaminationFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalExaminationFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalExaminationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
