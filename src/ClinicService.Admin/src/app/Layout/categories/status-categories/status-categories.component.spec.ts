import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusCategoriesComponent } from './status-categories.component';

describe('StatusCategoriesComponent', () => {
  let component: StatusCategoriesComponent;
  let fixture: ComponentFixture<StatusCategoriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatusCategoriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatusCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
