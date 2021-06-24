import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { ClinicBranch } from '@app/shared/models';
import { ClinicBranchesService, ErrorsService, NotificationsService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-clinic-branch-form',
  templateUrl: './clinic-branch-form.component.html',
  styleUrls: ['./clinic-branch-form.component.css']
})
export class ClinicBranchFormComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'address': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'phoneNumber': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private clinicBranchesService: ClinicBranchesService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
      'address': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'phoneNumber': new FormControl('', Validators.compose([
        Validators.required,
      ])),
    });

    if (this.entityId) {
      this.loadClinicBranchById(this.entityId);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadClinicBranchById(id: number): void {
    this.blockedPanel = true;

    this.subscription.add(this.clinicBranchesService.getDetail(id).subscribe((res: ClinicBranch) => {
      this.entityForm.setValue({
        name: res.name,
        address: res.address,
        phoneNumber: res.phoneNumber
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
      this.subscription.add(this.clinicBranchesService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.clinicBranchesService.add(this.entityForm.getRawValue()).subscribe(() => {
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
