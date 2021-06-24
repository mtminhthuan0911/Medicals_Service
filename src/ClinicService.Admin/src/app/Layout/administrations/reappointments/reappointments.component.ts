import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesConstant, SystemsConstant } from '@app/shared/constants';
import { MedicalExamination, Pagination, Reappointment } from '@app/shared/models';
import { ErrorsService, MedicalExaminationsService, NotificationsService, ReappointmentsService } from '@app/shared/services';
import { ConfirmationService, DialogService, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';
import { ReappointmentChangeStatusFormComponent } from './components/reappointment-change-status-form/reappointment-change-status-form.component';
import { ReappointmentFormComponent } from './components/reappointment-form/reappointment-form.component';

@Component({
  selector: 'app-reappointments',
  templateUrl: './reappointments.component.html',
  styleUrls: ['./reappointments.component.css']
})
export class ReappointmentsComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  public ref: DynamicDialogRef;

  public fromMedicalExaminationId = this.activatedRoute.snapshot.params.fromMedicalExaminationId ? +this.activatedRoute.snapshot.params.fromMedicalExaminationId : null;

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
  public selectedItem: Reappointment = null;

  public statusIdCompleted = SystemsConstant.STATUS_ID_COMPLETED;
  
  constructor(
    private reappointmentsService: ReappointmentsService,
    private medicalExaminationsService: MedicalExaminationsService,
    private dialogService: DialogService,
    private notificationsService: NotificationsService,
    private confirmationService: ConfirmationService,
    private errorsService: ErrorsService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.loadData();
    this.onShowAddedHasMedicalExaminationModal();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  //operate data methods
  loadData(): void {
    this.blockedPanel = true;
    this.subscription.add(this.reappointmentsService.getSearch(this.q, this.page, this.limit)
      .subscribe((res: Pagination<Reappointment>) => {
        this.processLoadData(res);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  processLoadData(res: Pagination<Reappointment>): void {
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
    this.ref = this.dialogService.open(ReappointmentFormComponent, {
      header: 'Thêm mới phiếu tái hẹn',
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  onShowAddedHasMedicalExaminationModal() {
    if (this.fromMedicalExaminationId) {
      this.subscription.add(this.medicalExaminationsService.getDetail(this.fromMedicalExaminationId).subscribe((res: MedicalExamination) => {
        const data = {
          fromMedicalExaminationId: this.fromMedicalExaminationId,
          patientId: res.patientId
        };

        this.ref = this.dialogService.open(ReappointmentFormComponent, {
          header: 'Thêm mới phiếu tái hẹn',
          data: data
        });

        this.ref.onClose.subscribe(() => {
          this.loadData();
          this.selectedItem = null;
        });
      }));
    }
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = {
      entityId: this.selectedItem.id,
    };

    this.ref = this.dialogService.open(ReappointmentFormComponent, {
      header: 'Cập nhật phiếu tái hẹn',
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
      message: 'Bạn có chắc chắn xoá phiếu tái hẹn này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.reappointmentsService.delete(id).subscribe(() => {
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

    this.ref = this.dialogService.open(ReappointmentChangeStatusFormComponent, {
      header: 'Cập nhật tình trạng phiếu tái hẹn',
      data: data
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  goToMedicalExamination(reappointmentId: number, patientId: string) {
    this.router.navigate(['administrations/medical-examination-form/from-reappointment', reappointmentId, patientId]);
  }

  goToEditMedicalExamination(reappointmentId: number) {
    this.subscription.add(this.medicalExaminationsService.getDetailFromReappointmentId(reappointmentId).subscribe(res => {
      this.router.navigate(['administrations/medical-examination-form', res.id]);
    }));
  }
}
