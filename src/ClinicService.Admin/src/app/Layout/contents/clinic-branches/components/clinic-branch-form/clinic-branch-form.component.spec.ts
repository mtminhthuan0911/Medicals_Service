import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClinicBranchFormComponent } from './clinic-branch-form.component';

describe('ClinicBranchFormComponent', () => {
  let component: ClinicBranchFormComponent;
  let fixture: ComponentFixture<ClinicBranchFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClinicBranchFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClinicBranchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
