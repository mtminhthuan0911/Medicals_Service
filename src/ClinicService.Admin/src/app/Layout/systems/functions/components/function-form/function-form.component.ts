import { Component, OnInit } from '@angular/core';
import { FunctionsService, NotificationsService, ErrorsService } from '@app/shared/services';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { Subscription } from 'rxjs';
import { Function } from '@app/shared/models';

@Component({
  selector: 'app-function-form',
  templateUrl: './function-form.component.html',
  styleUrls: ['./function-form.component.css']
})
export class FunctionFormComponent implements OnInit {

  public blockedPanel = false;

  private subscription = new Subscription();

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'url': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'sortOrder': [
      { type: 'minlength', message: 'SortOrder phải lớn hơn bằng 0' },
    ],
    'parentId': [
      { type: 'maxlength', message: 'Không được nhập quá 64 ký tự.' }
    ],
    'icon': [
      { type: 'maxlength', message: 'Không được nhập quá 32 ký tự.' }
    ],
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private functionsService: FunctionsService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'id': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
      'url': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
      'sortOrder': new FormControl('', Validators.compose([
        Validators.minLength(0),
      ])),
      'parentId': new FormControl('', Validators.compose([
        Validators.maxLength(64),
      ])),
      'icon': new FormControl('', Validators.compose([
        Validators.maxLength(32),
      ])),
    });

    if (this.entityId) {
      this.entityForm.controls['id'].disable({ onlySelf: true });
      this.loadFunctionById(this.entityId);
    }
  }

  loadFunctionById(id: string): void {
    this.blockedPanel = true;

    this.subscription.add(this.functionsService.getDetail(id).subscribe((res: Function) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        url: res.url,
        sortOrder: res.sortOrder,
        parentId: res.parentId,
        icon: res.icon
      });
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
    errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  saveData(): void {
    this.blockedPanel = true;

    if (this.entityId) {
      this.subscription.add(this.functionsService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      const entity: Function = this.entityForm.getRawValue();
      entity.sortOrder = parseInt(entity.sortOrder.toString(), 0);
      this.subscription.add(this.functionsService.add(entity).subscribe(() => {
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
