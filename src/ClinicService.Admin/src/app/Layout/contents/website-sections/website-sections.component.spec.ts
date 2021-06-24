import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WebsiteSectionsComponent } from './website-sections.component';

describe('WebsiteSectionsComponent', () => {
  let component: WebsiteSectionsComponent;
  let fixture: ComponentFixture<WebsiteSectionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WebsiteSectionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WebsiteSectionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
