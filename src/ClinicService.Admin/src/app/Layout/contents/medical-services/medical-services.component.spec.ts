import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalServicesComponent } from './medical-services.component';

describe('MedicalServicesComponent', () => {
  let component: MedicalServicesComponent;
  let fixture: ComponentFixture<MedicalServicesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicalServicesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicalServicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
