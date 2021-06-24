import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant, SystemsConstant } from '@app/shared/constants';
import { MedicalExamination, Reappointment, User } from '@app/shared/models';
import { ErrorsService, MedicalExaminationsService, NotificationsService, ReappointmentsService, UsersService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-reappointment-form',
  templateUrl: './reappointment-form.component.html',
  styleUrls: ['./reappointment-form.component.css']
})
export class ReappointmentFormComponent implements OnInit {
  private subscriptions: Subscription[] = [];

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  public fromPatientId = this.config.data ? this.config.data.patientId : null;
  public fromMedicalExaminationId = this.config.data ? this.config.data.fromMedicalExaminationId : null;

  public patients: User[] = [];
  public filteredPatients: User[];

  public fromMedicalExaminations: MedicalExamination[] = [];
  public filteredFromMedicalExaminations: MedicalExamination[];

  private statusCategoryId: string = SystemsConstant.STATUS_ID_RECEIVED;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'reappointmentDate': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'patientId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'fromMedicalExaminationId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private reappointmentsService: ReappointmentsService,
    private usersService: UsersService,
    private medicalExaminationsService: MedicalExaminationsService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'reappointmentDate': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'patientId': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'fromMedicalExaminationId': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'note': new FormControl('')
    });

    if (this.entityId) {
      this.loadReappointmentById();
    }
    else {
      this.loadCategories();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadCategories() {
    this.loadPatients();
    this.loadFromMedicalExaminations();
  }

  loadReappointmentById() {
    this.blockedPanel = true;

    this.subscriptions.push(this.reappointmentsService.getDetail(this.entityId).subscribe(res => {
      this.loadPatients(res.patientId);
      this.loadFromMedicalExaminations(res.fromMedicalExaminationId);

      this.entityForm.controls['reappointmentDate'].setValue(new Date(res.reappointmentDate));
      this.entityForm.controls['note'].setValue(res.note);

      this.statusCategoryId = res.statusCategoryId;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  loadPatients(patientId = this.fromPatientId) {
    this.subscriptions.push(this.usersService.getAllByRoleId(SystemsConstant.ROLE_ID_PATIENT).subscribe(res => {
      this.patients = res;
      if (patientId)
        this.entityForm.controls['patientId'].setValue(res.find(f => f.id == patientId));
    }));
  }

  loadFromMedicalExaminations(fromMedicalExaminationId = this.fromMedicalExaminationId) {
    this.subscriptions.push(this.medicalExaminationsService.getAll().subscribe(res => {
      this.fromMedicalExaminations = res;

      if (fromMedicalExaminationId)
        this.entityForm.controls['fromMedicalExaminationId'].setValue(res.find(f => f.id == fromMedicalExaminationId));
    }));
  }

  filterPatients(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.patients.length; i++) {
      let item = this.patients[i];
      if (item.firstName.toLowerCase().indexOf(query.toLowerCase()) == 0 ||
        item.lastName.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredPatients = filtered;
  }

  filterFromMedicalExaminations(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.fromMedicalExaminations.length; i++) {
      let item = this.fromMedicalExaminations[i];
      if (item.patientFullName.toLowerCase().indexOf(query.toLowerCase()) == 0 || item.id == +query) {
        filtered.push(item);
      }
    }

    this.filteredFromMedicalExaminations = filtered;
  }

  saveData(): void {
    this.blockedPanel = true;

    let formData = this.entityForm.getRawValue();
    let requestedData: Reappointment = {
      id: 0,
      createdDate: '',
      modifiedDate: '',
      patientFullName: '',
      statusCategoryId: this.statusCategoryId,
      statusCategoryName: '',
      reappointmentDate: formData.reappointmentDate,
      patientId: formData.patientId.id,
      note: formData.note,
      fromMedicalExaminationId: formData.fromMedicalExaminationId.id
    };

    if (this.entityId) {
      this.subscriptions.push(this.reappointmentsService.update(this.entityId, requestedData).subscribe(res => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    }
    else {
      this.subscriptions.push(this.reappointmentsService.add(requestedData).subscribe(res => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    }
  }
}
