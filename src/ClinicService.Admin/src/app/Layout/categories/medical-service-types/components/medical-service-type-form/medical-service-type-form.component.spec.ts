import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalServiceTypeFormComponent } from './medical-service-type-form.component';

describe('MedicalServiceTypeFormComponent', () => {
  let component: MedicalServiceTypeFormComponent;
  let fixture: ComponentFixture<MedicalServiceTypeFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalServiceTypeFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalServiceTypeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
