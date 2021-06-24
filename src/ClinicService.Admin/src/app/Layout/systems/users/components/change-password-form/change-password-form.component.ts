import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { UsersService, NotificationsService, ErrorsService } from '@app/shared/services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';

@Component({
  selector: 'app-change-password-form',
  templateUrl: './change-password-form.component.html',
  styleUrls: ['./change-password-form.component.css']
})
export class ChangePasswordFormComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  public entityId = this.config.data.entityId ? this.config.data.entityId : null;

  public entityForm: FormGroup;

  public blockedPanel = false;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'currentPassword': [
      { type: 'required', message: MessagesConstant.REQUIRED }
    ],
    'newPassword': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'minlength', message: 'Nhập ít nhất 6 ký tự.' },
      { type: 'pattern', message: 'Mật khẩu không đủ độ phức tạp.' }
    ],
    'confirmedPassword': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private usersService: UsersService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'currentPassword': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'newPassword': new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.pattern('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$')
      ])),
      'confirmedPassword': new FormControl('', Validators.compose([
        Validators.required
      ]))
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onSaveData(): void {
    this.blockedPanel = true;
    this.subscription.add(this.usersService.changePassword(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
      this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
      this.ref.close(true);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
}
