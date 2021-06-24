import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { StatusCategory } from '@app/shared/models';
import { ErrorsService, NotificationsService, StatusCategoriesService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-status-category-form',
  templateUrl: './status-category-form.component.html',
  styleUrls: ['./status-category-form.component.css']
})
export class StatusCategoryFormComponent implements OnInit, OnDestroy {
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
    private statusCategoriesService: StatusCategoriesService,
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
      'color': new FormControl(''),
      'parentId': new FormControl(''),
      'sortOrder': new FormControl('')
    });

    if (this.entityId) {
      this.loadStatusCategoryById(this.entityId);
      this.entityForm.controls['id'].disable();
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadStatusCategoryById(id: string): void {
    this.blockedPanel = true;

    this.subscription.add(this.statusCategoriesService.getDetail(id).subscribe((res: StatusCategory) => {
      console.log(res);

      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        color: res.color,
        parentId: res.parentId,
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
      this.subscription.add(this.statusCategoriesService.update(this.entityId, this.entityForm.getRawValue()).subscribe(() => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    } else {
      this.subscription.add(this.statusCategoriesService.add(this.entityForm.getRawValue()).subscribe(() => {
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
