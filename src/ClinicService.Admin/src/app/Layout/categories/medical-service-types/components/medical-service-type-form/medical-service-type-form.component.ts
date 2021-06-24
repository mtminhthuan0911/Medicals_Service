import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { MedicalServiceType } from '@app/shared/models';
import { ErrorsService, MedicalServiceTypesService, NotificationsService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-medical-service-type-form',
  templateUrl: './medical-service-type-form.component.html',
  styleUrls: ['./medical-service-type-form.component.css']
})
export class MedicalServiceTypeFormComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'id': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private medicalServiceTypesService: MedicalServiceTypesService,
    private formBuilder: FormBuilder,
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
      'icon': new FormControl(''),
      'sortOrder': new FormControl(''),
    });

    if (this.entityId) {
      this.loadMedicalServiceTypeById(this.entityId);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadMedicalServiceTypeById(id: string): void {
    this.blockedPanel = true;

    this.subscription.add(this.medicalServiceTypesService.getDetail(id).subscribe((res: MedicalServiceType) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        icon: res.icon,
        sortOrder: res.sortOrder
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
      this.subscription.add(this.medicalServiceTypesService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.medicalServiceTypesService.add(this.entityForm.getRawValue()).subscribe(() => {
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
