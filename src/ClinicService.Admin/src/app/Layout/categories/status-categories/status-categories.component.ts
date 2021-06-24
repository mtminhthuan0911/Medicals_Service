import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessagesConstant } from '@app/shared/constants';
import { StatusCategory } from '@app/shared/models';
import { ErrorsService, NotificationsService, StatusCategoriesService, UtilitiesService } from '@app/shared/services';
import { ConfirmationService, DialogService, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';
import { StatusCategoryFormComponent } from './components/status-category-form/status-category-form.component';

@Component({
  selector: 'app-status-categories',
  templateUrl: './status-categories.component.html',
  styleUrls: ['./status-categories.component.css']
})
export class StatusCategoriesComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  public ref: DynamicDialogRef;

  // Default
  public blockedPanel = false;

  public items: any[];
  public selectedItem: StatusCategory = null;

  constructor(
    private statusCategoriesService: StatusCategoriesService,
    private utilitiesService: UtilitiesService,
    private dialogService: DialogService,
    private notificationsService: NotificationsService,
    private confirmationService: ConfirmationService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  //operate data methods
  loadData(): void {
    this.blockedPanel = true;
    this.subscription.add(this.statusCategoriesService.getAll()
      .subscribe(res => {
        const statusCategoriesTree = this.utilitiesService.UnflatteringForTree(res);
        this.items = statusCategoriesTree;
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  //event methods
  onShowAddedModal(): void {
    this.ref = this.dialogService.open(StatusCategoryFormComponent, {
      header: 'Thêm mới tình trạng',
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = {
      entityId: this.selectedItem.id,
    };

    this.ref = this.dialogService.open(StatusCategoryFormComponent, {
      header: 'Cập nhật tình trạng',
      data: data,
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  onConfirmDelete(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá tình trạng này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.statusCategoriesService.delete(id).subscribe(() => {
          this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
          this.loadData();
          this.selectedItem = null;
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
          errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => { this.blockedPanel = false; }, 1000);
          }));
      }
    });
  }
}
