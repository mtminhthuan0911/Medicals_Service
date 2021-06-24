import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusCategoryFormComponent } from './status-category-form.component';

describe('StatusCategoryFormComponent', () => {
  let component: StatusCategoryFormComponent;
  let fixture: ComponentFixture<StatusCategoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatusCategoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatusCategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
