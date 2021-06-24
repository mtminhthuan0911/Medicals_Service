import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessagesConstant, SystemsConstant } from '@app/shared/constants';
import { Appointment, Pagination } from '@app/shared/models';
import { AppointmentsService, ErrorsService, MedicalExaminationsService, NotificationsService } from '@app/shared/services';
import { ConfirmationService, DialogService, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';
import { AppointmentChangeStatusFormComponent } from './components/appointment-change-status-form/appointment-change-status-form.component';
import { AppointmentFormComponent } from './components/appointment-form/appointment-form.component';

@Component({
  selector: 'app-appointments',
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent implements OnInit, OnDestroy {
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
  public selectedItem: Appointment = null;

  public statusIdCompleted = SystemsConstant.STATUS_ID_COMPLETED;

  constructor(
    private appointmentsService: AppointmentsService,
    private medicalExaminationsService: MedicalExaminationsService,
    private dialogService: DialogService,
    private notificationsService: NotificationsService,
    private confirmationService: ConfirmationService,
    private errorsService: ErrorsService,
    private router: Router,
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
    this.subscription.add(this.appointmentsService.getSearch(this.q, this.page, this.limit)
      .subscribe((res: Pagination<Appointment>) => {
        this.processLoadData(res);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  processLoadData(res: Pagination<Appointment>): void {
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
    this.ref = this.dialogService.open(AppointmentFormComponent, {
      header: 'Thêm mới phiếu hẹn',
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

    this.ref = this.dialogService.open(AppointmentFormComponent, {
      header: 'Cập nhật phiếu hẹn',
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
      message: 'Bạn có chắc chắn xoá phiếu hẹn này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.appointmentsService.delete(id).subscribe(() => {
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

    this.ref = this.dialogService.open(AppointmentChangeStatusFormComponent, {
      header: 'Cập nhật tình trạng phiếu hẹn',
      data: data
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  goToMedicalExamination(appointmentId: number, patientId: string) {
    this.router.navigate(['administrations/medical-examination-form/from-appointment', appointmentId, patientId]);
  }

  goToEditMedicalExamination(appointmentId) {
    this.subscription.add(this.medicalExaminationsService.getDetailFromAppointmentId(appointmentId).subscribe(res => {
      this.router.navigate(['administrations/medical-examination-form', res.id]);
    }));
  }
}
