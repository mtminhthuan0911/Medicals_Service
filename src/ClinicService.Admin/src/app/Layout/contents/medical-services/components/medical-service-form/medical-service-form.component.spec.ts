import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalServiceFormComponent } from './medical-service-form.component';

describe('MedicalServiceFormComponent', () => {
  let component: MedicalServiceFormComponent;
  let fixture: ComponentFixture<MedicalServiceFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalServiceFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalServiceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
