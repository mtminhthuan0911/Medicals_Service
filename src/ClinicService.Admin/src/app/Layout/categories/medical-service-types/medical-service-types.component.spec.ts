import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalServiceTypesComponent } from './medical-service-types.component';

describe('MedicalServiceTypesComponent', () => {
  let component: MedicalServiceTypesComponent;
  let fixture: ComponentFixture<MedicalServiceTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalServiceTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalServiceTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
