import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesAssignFormComponent } from './roles-assign-form.component';

describe('RolesAssignFormComponent', () => {
  let component: RolesAssignFormComponent;
  let fixture: ComponentFixture<RolesAssignFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RolesAssignFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RolesAssignFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
