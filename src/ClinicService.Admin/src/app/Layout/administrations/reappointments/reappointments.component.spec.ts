import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReappointmentsComponent } from './reappointments.component';

describe('ReappointmentsComponent', () => {
  let component: ReappointmentsComponent;
  let fixture: ComponentFixture<ReappointmentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReappointmentsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReappointmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
