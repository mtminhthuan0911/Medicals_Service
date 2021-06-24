import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClinicBranchesComponent } from './clinic-branches.component';

describe('ClinicBranchesComponent', () => {
  let component: ClinicBranchesComponent;
  let fixture: ComponentFixture<ClinicBranchesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClinicBranchesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClinicBranchesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
