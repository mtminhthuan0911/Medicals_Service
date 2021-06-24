import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { StatusCategory } from '@app/shared/models';
import { AppointmentsService, ErrorsService, NotificationsService, StatusCategoriesService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef, SelectItem, SelectItemGroup } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-appointment-change-status-form',
  templateUrl: './appointment-change-status-form.component.html',
  styleUrls: ['./appointment-change-status-form.component.css']
})
export class AppointmentChangeStatusFormComponent implements OnInit, OnDestroy {
  public subscription: Subscription = new Subscription();

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public statusCategoryId = this.config.data ? this.config.data.statusCategoryId : null;
  public entityForm: FormGroup;

  public groupedStatusCategories: SelectItemGroup[];

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private formBuilder: FormBuilder,
    private statusCategoriesService: StatusCategoriesService,
    private appointmentsService: AppointmentsService,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.loadStatusCategories();

    this.entityForm = this.formBuilder.group({
      'statusCategoryId': new FormControl('')
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadStatusCategories() {
    this.blockedPanel = true;

    this.subscription.add(this.statusCategoriesService.getAll().subscribe((res: StatusCategory[]) => {
      this.groupedStatusCategories = this.groupStatusCategories(res);
      let statusCategory = res.find(i => i.id == this.statusCategoryId);
      let selectedItem: SelectItem = { label: statusCategory.name, value: statusCategory.id };

      this.entityForm.setValue({
        statusCategoryId: selectedItem.value
      });

      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  saveData() {
    if (this.entityId) {
      let formStatusCategoryId = this.entityForm.getRawValue().statusCategoryId;
      this.subscription.add(this.appointmentsService.updateStatus(this.entityId, formStatusCategoryId).subscribe(res => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }))
    }
  }

  private groupStatusCategories(list: StatusCategory[]) {
    let groupedList: SelectItemGroup[] = [];

    list.forEach(item => {
      if (item.parentId === null) {
        let groupedItem: SelectItemGroup = {
          label: item.name,
          items: list.filter(f => f.parentId == item.id).map(m => { return { label: m.name, value: m.id } })
        };
        groupedList.push(groupedItem);
      }
    })

    return groupedList;
  }
}
