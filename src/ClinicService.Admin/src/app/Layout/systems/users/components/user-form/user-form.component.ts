import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessagesConstant } from '@app/shared/constants';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { UsersService, NotificationsService, ErrorsService } from '@app/shared/services';
import { User } from '@app/shared/models';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  public hideControl: string;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'userName': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'minlength', message: 'Nhập ít nhất 2 ký tự.' },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'email': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' },
      { type: 'pattern', message: 'Email không đúng định dạng.' }
    ],
    'lastName': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'minlength', message: 'Nhập ít nhất 2 ký tự.' },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'firstName': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'minlength', message: 'Nhập ít nhất 2 ký tự.' },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'address': [
      { type: 'maxlength', message: 'Không được nhập quá 1024 ký tự.' }
    ],
    'phoneNumber': [
      { type: 'minlength', message: 'Nhập ít nhất 9 ký tự.' },
      { type: 'maxlength', message: 'Không được nhập quá 16 ký tự.' }
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
      'userName': new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(256)
      ])),
      'email': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256),
        Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')
      ])),
      'lastName': new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(256)
      ])),
      'firstName': new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(256)
      ])),
      'dateOfBirth': new FormControl(''),
      'phoneNumber': new FormControl('', Validators.compose([
        Validators.minLength(10),
        Validators.maxLength(16)
      ])),
      'address': new FormControl('', Validators.compose([
        Validators.maxLength(1024)
      ]))
    });

    // Disabling controls
    if (this.entityId) {
      this.loadUserDetail(this.entityId);
      this.entityForm.controls['userName'].disable({ onlySelf: true });
      this.entityForm.controls['email'].disable({ onlySelf: true });
      this.hideControl = 'd-block';
    } else {
      this.hideControl = 'd-none';
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadUserDetail(id: string) {
    this.blockedPanel = true;
    this.usersService.getDetail(id).subscribe((res: User) => {
      const dateOfBirth: Date = new Date(res.dateOfBirth);
      this.entityForm.setValue({
        userName: res.userName,
        email: res.email,
        lastName: res.lastName,
        firstName: res.firstName,
        dateOfBirth: dateOfBirth,
        phoneNumber: res.phoneNumber,
        address: res.address,
      });

      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      });
  }

  saveData(): void {
    this.blockedPanel = true;
    if (this.entityId) {
      console.log(this.entityForm.getRawValue());
      this.subscription.add(this.usersService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.usersService.add(this.entityForm.getRawValue()).subscribe(() => {
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
