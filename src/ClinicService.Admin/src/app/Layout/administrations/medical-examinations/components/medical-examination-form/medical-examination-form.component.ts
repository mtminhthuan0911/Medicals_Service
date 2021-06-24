import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesConstant, SystemsConstant } from '@app/shared/constants';
import { Appointment, MedicalExamination, MedicalExaminationDetail, MedicalExaminationFull, Prescription, Reappointment, User } from '@app/shared/models';
import { AppointmentsService, AuthService, ErrorsService, MedicalExaminationsService, NotificationsService, ReappointmentsService, UsersService, UtilitiesService } from '@app/shared/services';
import { environment } from '@environments/environment';
import { ConfirmationService, DialogService, DynamicDialogRef } from 'primeng';
import { forkJoin, Subscription } from 'rxjs';
import { MedicalExaminationDetailFormComponent } from '../medical-examination-detail-form/medical-examination-detail-form.component';
import { PrescriptionFormComponent } from '../prescription-form/prescription-form.component';

@Component({
  selector: 'app-medical-examination-form',
  templateUrl: './medical-examination-form.component.html',
  styleUrls: ['./medical-examination-form.component.css']
})
export class MedicalExaminationFormComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];
  public ref: DynamicDialogRef;

  public blockedPanel = false;

  public fromAppointmentId = this.activatedRoute.snapshot.params.appointmentId ? +this.activatedRoute.snapshot.params.appointmentId : null;
  public fromReappointmentId = this.activatedRoute.snapshot.params.reappointmentId ? +this.activatedRoute.snapshot.params.reappointmentId : null;
  public fromPatientId = this.activatedRoute.snapshot.params.patientId ? this.activatedRoute.snapshot.params.patientId : null;

  public entityId = this.activatedRoute.snapshot.params.id ? +this.activatedRoute.snapshot.params.id : null;
  public entityForm: FormGroup;

  public attachments: any[] = [];
  public selectedFiles: File[] = [];

  public serverPath = environment.apiUrl;

  public patients: User[] = [];
  public filteredPatients: User[];

  public appointments: Appointment[] = [];
  public filteredAppointments: Appointment[];

  public reappointments: Reappointment[] = [];
  public filteredReappointments: Reappointment[];

  public details: MedicalExaminationDetail[] = [];
  public selectedDetail: MedicalExaminationDetail;

  public prescriptions: Prescription[] = [];
  public selectedPrescription: Prescription;

  public statusCategoryId: string = SystemsConstant.STATUS_ID_COMPLETED;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'patientId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private usersService: UsersService,
    private medicalExaminationsService: MedicalExaminationsService,
    private appointmentsService: AppointmentsService,
    private reappointmentsService: ReappointmentsService,
    private utilitiesService: UtilitiesService,
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService,
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'patientId': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'appointmentId': new FormControl(''),
      'reappointmentId': new FormControl(''),
    });

    if (this.entityId) {
      this.loadMedicalExaminationById();
    }
    else {
      this.loadCategories();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  // operate data methods
  loadCategories() {
    this.loadPatients();
    this.loadAppointments();
    this.loadReappointments();
  }

  loadPatients(patientId = this.fromPatientId) {
    this.subscriptions.push(this.usersService.getAllByRoleId(SystemsConstant.ROLE_ID_PATIENT).subscribe(res => {
      this.patients = res;
      if (patientId)
        this.entityForm.controls['patientId'].setValue(res.find(f => f.id == patientId));
    }));
  }

  loadAppointments(appointmentId = this.fromAppointmentId) {
    this.subscriptions.push(this.appointmentsService.getAll().subscribe(res => {
      this.appointments = res;
      if (appointmentId)
        this.entityForm.controls['appointmentId'].setValue(res.find(f => f.id == appointmentId));
    }));
  }

  loadReappointments(reappointmentId = this.fromReappointmentId) {
    this.subscriptions.push(this.reappointmentsService.getAll().subscribe(res => {
      this.reappointments = res;
      if (reappointmentId)
        this.entityForm.controls['reappointmentId'].setValue(res.find(f => f.id == reappointmentId));
    }));
  }

  loadMedicalExaminationById(): void {
    this.blockedPanel = true;
    this.subscriptions.push(this.medicalExaminationsService.getDetail(this.entityId).subscribe((res: MedicalExamination) => {
      this.loadPatients(res.patientId);
      this.loadAppointments(res.appointmentId);
      this.loadReappointments(res.reappointmentId)

      this.statusCategoryId = res.statusCategoryId;
      this.details = res.details;
      this.prescriptions = res.prescriptions;
      this.attachments = res.attachments;

      setTimeout(() => this.blockedPanel = false, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => this.blockedPanel = false, 1000);
    }));
  }

  loadDetails(id: number): void {
    this.subscriptions.push(this.medicalExaminationsService.getDetails(id).subscribe(res => {
      this.details = res;
    }, errors => {
      this.errorsService.notifyErrors(errors);
    }))
  }

  loadPrescriptions(id: number): void {
    this.subscriptions.push(this.medicalExaminationsService.getPrescriptions(id).subscribe(res => {
      this.prescriptions = res;
    }, errors => {
      this.errorsService.notifyErrors(errors);
    }))
  }

  filterPatients(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.patients.length; i++) {
      let item = this.patients[i];
      if (item.firstName.toLowerCase().indexOf(query.toLowerCase()) == 0 || item.lastName.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredPatients = filtered;
  }

  filterAppointments(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.appointments.length; i++) {
      let item = this.appointments[i];
      if (item.id === +query || item.patientFullName.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredAppointments = filtered;
  }

  filterReappointments(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.reappointments.length; i++) {
      let item = this.reappointments[i];
      if (item.id === +query || item.patientFullName.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredReappointments = filtered;
  }

  // event methods
  onShowAddedDetailModal(): void {
    this.ref = this.dialogService.open(MedicalExaminationDetailFormComponent, {
      header: 'Thêm mới chuẩn đoán',
    });

    this.ref.onClose.subscribe((data: MedicalExaminationDetail) => {
      //this.loadData();
      if (data) {
        this.details.push({
          id: 0,
          diagnostic: data.diagnostic,
          treatment: data.treatment,
          createdDate: null,
          modifiedDate: null,
          doctorId: this.authService.profile.sub,
          medicalExaminationId: 0
        });

        if (this.entityId) {
          this.blockedPanel = true;

          data.doctorId = this.authService.profile.sub;
          data.medicalExaminationId = this.entityId;

          this.subscriptions.push(this.medicalExaminationsService.addDetail(this.entityId, data).subscribe(res => {
            this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
            this.loadDetails(this.entityId);
            setTimeout(() => this.blockedPanel = false, 1000);
          }, errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => this.blockedPanel = false, 1000);
          }));
        }

        this.selectedDetail = null;
      }
    });
  }

  onShowEditedDetailModal(): void {
    if (this.selectedDetail === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.ref = this.dialogService.open(MedicalExaminationDetailFormComponent, {
      header: 'Cập nhật chuẩn đoán',
      data: this.selectedDetail
    });

    this.ref.onClose.subscribe((data: MedicalExaminationDetail) => {
      if (data) {
        this.selectedDetail.diagnostic = data.diagnostic;
        this.selectedDetail.treatment = data.treatment;
        this.selectedDetail.doctorId = data.doctorId;
        this.selectedDetail.medicalExaminationId = data.medicalExaminationId;

        if (this.entityId) {
          this.blockedPanel = true;

          this.subscriptions.push(this.medicalExaminationsService.updateDetail(this.entityId, data.id, data).subscribe(res => {
            this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
            setTimeout(() => this.blockedPanel = false, 1000);
          }, errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => this.blockedPanel = false, 1000);
          }))
        }

        this.selectedDetail = null;
      }
    });
  }

  onConfirmDeleteDetail(): void {
    if (this.selectedDetail === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá chuẩn đoán này?',
      accept: () => {
        this.blockedPanel = true;

        let id = this.details.indexOf(this.selectedDetail);
        if (id > -1) {
          this.details.splice(id, 1);

          if (this.entityId) {
            this.subscriptions.push(this.medicalExaminationsService.deleteDetail(this.entityId, this.selectedDetail.id).subscribe(res => {
              this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
            }, errors => {
              this.errorsService.notifyErrors(errors);
            }))
          }
        }

        setTimeout(() => this.blockedPanel = false, 1000);
      }
    });
  }

  onShowAddedPrescriptionModal(): void {
    this.ref = this.dialogService.open(PrescriptionFormComponent, {
      header: 'Thêm mới thuốc',
    });

    this.ref.onClose.subscribe((data: Prescription) => {
      if (data) {
        this.prescriptions.push({
          id: 0,
          name: data.name,
          subname: data.subname,
          quantity: data.quantity,
          availableQuantity: data.availableQuantity,
          total: data.total,
          take: data.take,
          isMorning: data.isMorning,
          isAfternoon: data.isAfternoon,
          isEvening: data.isEvening,
          note: data.note,
          createdDate: null,
          modifiedDate: null,
          doctorId: this.authService.profile.sub,
          medicalExaminationId: 0
        });

        if (this.entityId) {
          this.blockedPanel = true;

          data.doctorId = this.authService.profile.sub;
          data.medicalExaminationId = this.entityId;

          this.subscriptions.push(this.medicalExaminationsService.addPrescription(this.entityId, data).subscribe(res => {
            this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
            this.loadPrescriptions(this.entityId);
            setTimeout(() => this.blockedPanel = false, 1000);
          }, errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => this.blockedPanel = false, 1000);
          }))
        }

        this.selectedPrescription = null;
      }
    });
  }

  onShowEditedPrescriptionModal(): void {
    if (this.selectedPrescription === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.ref = this.dialogService.open(PrescriptionFormComponent, {
      header: 'Cập nhật thuốc',
      data: this.selectedPrescription
    });

    this.ref.onClose.subscribe((data: Prescription) => {
      //this.loadData();
      if (data) {
        this.selectedPrescription.name = data.name;
        this.selectedPrescription.subname = data.subname;
        this.selectedPrescription.quantity = data.quantity;
        this.selectedPrescription.availableQuantity = data.availableQuantity;
        this.selectedPrescription.total = data.total;
        this.selectedPrescription.take = data.take;
        this.selectedPrescription.isMorning = data.isMorning;
        this.selectedPrescription.isAfternoon = data.isAfternoon;
        this.selectedPrescription.isEvening = data.isEvening;
        this.selectedPrescription.note = data.note;
        this.selectedPrescription.doctorId = data.doctorId;
        this.selectedPrescription.medicalExaminationId = data.medicalExaminationId;

        if (this.entityId) {
          this.blockedPanel = true;

          this.subscriptions.push(this.medicalExaminationsService.updatePrescription(data.medicalExaminationId, data.id, this.selectedPrescription).subscribe(res => {
            this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
            setTimeout(() => this.blockedPanel = false, 1000);
          }, errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => this.blockedPanel = false, 1000);
          }));
        }

        this.selectedPrescription = null;
      }
    });
  }

  onConfirmDeletePrescription(): void {
    if (this.selectedPrescription === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá thuốc này?',
      accept: () => {
        this.blockedPanel = true;

        let id = this.prescriptions.indexOf(this.selectedPrescription);
        if (id > -1) {
          this.prescriptions.splice(id, 1);

          if (this.entityId) {
            this.subscriptions.push(this.medicalExaminationsService.deletePrescription(this.entityId, this.selectedPrescription.id).subscribe(res => {
              this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
              setTimeout(() => this.blockedPanel = false, 1000);
            }, errors => {
              this.errorsService.notifyErrors(errors);
              setTimeout(() => this.blockedPanel = false, 1000);
            }))
          }
        }

        setTimeout(() => this.blockedPanel = false, 1000);
      }
    });
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
    this.subscriptions.push(this.medicalExaminationsService.deleteAttachment(this.entityId, attachmentId)
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

  saveData(goToReappointment: boolean = false): void {
    this.blockedPanel = true;

    this.details.forEach(item => {
      item.doctorId = this.authService.profile.sub;
    });

    this.prescriptions.forEach(item => {
      item.doctorId = this.authService.profile.sub;
    });

    if (this.entityId) {
      let rawFormData = this.entityForm.getRawValue();

      let updatedData: MedicalExamination = {
        patientId: rawFormData.patientId.id,
        appointmentId: rawFormData.appointmentId.id,
        reappointmentId: rawFormData.reappointmentId.id,
        statusCategoryId: this.statusCategoryId,
        createdDate: null,
        details: null,
        id: null,
        modifiedDate: null,
        patientFullName: null,
        prescriptions: null,
        statusCategoryName: null,
        attachments: null
      };

      const formData = this.utilitiesService.ToFormData(updatedData);
      this.selectedFiles.forEach(file => formData.append('attachments', file, file.name));

      this.subscriptions.push(this.medicalExaminationsService.update(this.entityId, formData).subscribe(() => {
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => this.blockedPanel = false, 1000);
        },
        () => {
          this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
          if (goToReappointment === false)
            this.goBack();
          else {
            this.router.navigate(['administrations/re-appointments/from-medical-examination/', this.entityId]);
          }
        }));
    }
    else {
      let rawFormData = this.entityForm.getRawValue();

      let requestData: MedicalExaminationFull = {
        patientId: rawFormData.patientId.id,
        appointmentId: rawFormData.appointmentId.id,
        reappointmentId: rawFormData.reappointmentId.id,
        statusCategoryId: this.statusCategoryId,
        detailRequestModels: JSON.stringify(this.details),
        prescriptionRequestModels: JSON.stringify(this.prescriptions),
        attachments: null
      };

      const formData = this.utilitiesService.ToFormData(requestData);
      this.selectedFiles.forEach(file => formData.append('attachments', file, file.name));

      this.subscriptions.push(this.medicalExaminationsService.addFull(formData).subscribe((res: any) => {
        if (goToReappointment && res.body) {
          this.router.navigate(['administrations/re-appointments/from-medical-examination/', res.body.id]);
        }
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => this.blockedPanel = false, 1000);
        },
        () => {
          this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
          if (goToReappointment === false)
            this.goBack();
        }));
    }
  }

  goBack(): void {
    this.router.navigateByUrl('/administrations/medical-examinations');
  }
}
