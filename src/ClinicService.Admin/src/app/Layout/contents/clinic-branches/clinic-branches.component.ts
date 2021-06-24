import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessagesConstant } from '@app/shared/constants';
import { ClinicBranch, Pagination } from '@app/shared/models';
import { ClinicBranchesService, ErrorsService, NotificationsService } from '@app/shared/services';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';
import { ClinicBranchFormComponent } from './components/clinic-branch-form/clinic-branch-form.component';

@Component({
  selector: 'app-clinic-branches',
  templateUrl: './clinic-branches.component.html',
  styleUrls: ['./clinic-branches.component.css']
})
export class ClinicBranchesComponent implements OnInit, OnDestroy {
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
  public selectedItem: ClinicBranch = null;

  constructor(
    private clinicBranchesService: ClinicBranchesService,
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
    this.subscription.add(this.clinicBranchesService.getSearch(this.q, this.page, this.limit)
      .subscribe((res: Pagination<ClinicBranch>) => {
        this.processLoadData(res);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  processLoadData(res: Pagination<ClinicBranch>): void {
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
    this.ref = this.dialogService.open(ClinicBranchFormComponent, {
      header: 'Thêm mới chi nhánh',
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

    this.ref = this.dialogService.open(ClinicBranchFormComponent, {
      header: 'Cập nhật chi nhánh',
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
      message: 'Bạn có chắc chắn xoá chi nhánh này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.clinicBranchesService.delete(id).subscribe(() => {
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
