import { Component, OnInit, OnDestroy, EventEmitter } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { RolesService, NotificationsService, ErrorsService } from '@app/shared/services';
import { Subscription } from 'rxjs';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Role } from '@app/shared/models';
import { MessagesConstant } from '@app/shared/constants';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.css']
})
export class RoleFormComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  public hideControlId: string = '';

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private rolesService: RolesService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {

    this.entityForm = this.formBuilder.group({
      'id': new FormControl('', Validators.required),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
    });

    if (this.entityId) {
      this.loadRoleById(this.entityId);
      this.hideControlId = 'd-none';
    }
      
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadRoleById(id: string): void {
    this.blockedPanel = true;

    this.subscription.add(this.rolesService.getDetail(id).subscribe((res: Role) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name
      });
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  saveData(): void {
    this.blockedPanel = true;

    if (this.entityId) {
      this.subscription.add(this.rolesService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.rolesService.add(this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    }
  }
}
