import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WebsiteSectionFormComponent } from './website-section-form.component';

describe('WebsiteSectionFormComponent', () => {
  let component: WebsiteSectionFormComponent;
  let fixture: ComponentFixture<WebsiteSectionFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WebsiteSectionFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WebsiteSectionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
