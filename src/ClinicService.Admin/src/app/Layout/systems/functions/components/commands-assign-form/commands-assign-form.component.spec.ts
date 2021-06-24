import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommandsAssignFormComponent } from './commands-assign-form.component';

describe('CommandsAssignFormComponent', () => {
  let component: CommandsAssignFormComponent;
  let fixture: ComponentFixture<CommandsAssignFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommandsAssignFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommandsAssignFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
