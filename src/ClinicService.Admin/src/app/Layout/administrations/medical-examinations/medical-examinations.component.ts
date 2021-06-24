import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { MedicalExamination, Pagination } from '@app/shared/models';
import { ErrorsService, MedicalExaminationsService, NotificationsService } from '@app/shared/services';
import { ConfirmationService, DialogService, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';
import { MedicalExaminationChangeStatusFormComponent } from './components/medical-examination-change-status-form/medical-examination-change-status-form.component';

@Component({
  selector: 'app-medical-examinations',
  templateUrl: './medical-examinations.component.html',
  styleUrls: ['./medical-examinations.component.css']
})
export class MedicalExaminationsComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  public ref: DynamicDialogRef;

  // Default
  public blockedPanel = false;

  /**
   * Paging
   */
  public page = 1;
  public limit = 10;
  public totalRecords: number;
  public q = '';

  public items: any[];
  public selectedItem: MedicalExamination = null;

  constructor(
    private dialogService: DialogService,
    private router: Router,
    private medicalExaminationsService: MedicalExaminationsService,
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
    this.subscription.add(this.medicalExaminationsService.getSearch(this.q, this.page, this.limit)
      .subscribe((res: Pagination<MedicalExamination>) => {
        this.processLoadData(res);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  processLoadData(res: Pagination<MedicalExamination>): void {
    this.items = res.items;
    this.page = this.page;
    this.limit = this.limit;
    this.totalRecords = res.totalRecords;
  }

  //event methods
  onPageChanged(event: any): void {
    this.page = event.page + 1;
    this.limit = event.rows;
    this.loadData();
  }

  onShowAddedModal(): void {
    this.router.navigateByUrl('/administrations/medical-examination-form/');
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.router.navigateByUrl('/administrations/medical-examination-form/' + this.selectedItem.id);
  }

  onConfirmDelete(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá phiếu kết quả này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.medicalExaminationsService.delete(id).subscribe(() => {
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

  onShowChangeStatusModal(id: string) {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = {
      entityId: this.selectedItem.id,
      statusCategoryId: this.selectedItem.statusCategoryId
    };

    this.ref = this.dialogService.open(MedicalExaminationChangeStatusFormComponent, {
      header: 'Cập nhật tình trạng',
      data: data
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }
}
