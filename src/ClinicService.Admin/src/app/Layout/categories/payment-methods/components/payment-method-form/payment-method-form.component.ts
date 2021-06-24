import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { PaymentMethod } from '@app/shared/models';
import { ErrorsService, NotificationsService, PaymentMethodsService, UtilitiesService } from '@app/shared/services';
import { environment } from '@environments/environment';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-payment-method-form',
  templateUrl: './payment-method-form.component.html',
  styleUrls: ['./payment-method-form.component.css']
})
export class PaymentMethodFormComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  public attachments: any[] = [];
  public selectedFiles: File[] = [];

  public serverPath = environment.apiUrl;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'id': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private paymentMethodsService: PaymentMethodsService,
    private formBuilder: FormBuilder,
    private utilitiesService: UtilitiesService,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'id': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'name': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'sortOrder': new FormControl('')
    });

    if (this.entityId) {
      this.loadPaymentMethodById(this.entityId);
      this.entityForm.controls['id'].disable();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadPaymentMethodById(id: string): void {
    this.blockedPanel = true;

    this.subscriptions.push(this.paymentMethodsService.getDetail(id).subscribe((res: PaymentMethod) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        sortOrder: res.sortOrder
      });

      this.attachments = res.attachments;

      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  selectAttachments($event) {
    if ($event.currentFiles) {
      $event.currentFiles.forEach(file => {
        this.selectedFiles.push(file);
      });
    }
  }

  removeAttachments($event) {
    if ($event.file)
      this.selectedFiles.splice(this.selectedFiles.findIndex(item => item.name === $event.file.name), 1);
  }

  deleteAttachment(attachmentId) {
    this.blockedPanel = true;
    this.subscriptions.push(this.paymentMethodsService.deleteAttachment(this.entityId, attachmentId)
      .subscribe(() => {
        this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
        this.attachments.splice(this.attachments.findIndex(item => item.id === attachmentId), 1);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    return false;
  }

  saveData(): void {
    this.blockedPanel = true;

    const formData = this.utilitiesService.ToFormData(this.entityForm.getRawValue());
    this.selectedFiles.forEach(file => formData.append('attachments', file, file.name));

    if (this.entityId) {
      this.subscriptions.push(this.paymentMethodsService.update(this.entityId, formData).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscriptions.push(this.paymentMethodsService.add(formData).subscribe(() => {
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
