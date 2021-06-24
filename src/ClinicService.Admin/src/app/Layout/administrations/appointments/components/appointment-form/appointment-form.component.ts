import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant, SystemsConstant } from '@app/shared/constants';
import { Appointment, ClinicBranch, MedicalService, User } from '@app/shared/models';
import { AppointmentsService, ClinicBranchesService, ErrorsService, MedicalServicesService, NotificationsService, UsersService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { forkJoin, Subscription } from 'rxjs';

@Component({
  selector: 'app-appointment-form',
  templateUrl: './appointment-form.component.html',
  styleUrls: ['./appointment-form.component.css']
})
export class AppointmentFormComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  public blockedPanel = false;

  public entityId = this.config.data ? this.config.data.entityId : null;
  public entityForm: FormGroup;

  public medicalServices: MedicalService[] = [];
  public filteredMedicalServices: MedicalService[];

  public patients: User[] = [];
  public filteredPatients: User[];

  public clinicBranches: ClinicBranch[] = [];
  public filteredClinicBranches: ClinicBranch[];

  private statusCategoryId: string = SystemsConstant.STATUS_ID_RECEIVED;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'appointmentDate': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'patientId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'clinicBranchId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private appointmentsService: AppointmentsService,
    private medicalServicesService: MedicalServicesService,
    private clinicBranchesService: ClinicBranchesService,
    private usersService: UsersService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'appointmentDate': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'medicalServiceId': new FormControl(''),
      'patientId': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'clinicBranchId': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'note': new FormControl(''),
      'guessFullName': new FormControl(''),
      'guessPhoneNumber': new FormControl('')
    });

    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadAppointmentById(id: number) {
    this.blockedPanel = true;

    this.subscriptions.push(this.appointmentsService.getDetail(id).subscribe(res => {
      this.entityForm.setValue({
        appointmentDate: new Date(res.appointmentDate),
        medicalServiceId: this.medicalServices.find(f => f.id == res.medicalServiceId),
        patientId: this.patients.find(f => f.id == res.patientId),
        clinicBranchId: this.clinicBranches.find(f => f.id == res.clinicBranchId),
        note: res.note,
        guessFullName: res.guessFullName,
        guessPhoneNumber: res.guessPhoneNumber
      });

      this.statusCategoryId = res.statusCategoryId;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  loadCategories() {
    return forkJoin([
      this.medicalServicesService.getAll(),
      this.usersService.getAllByRoleId(SystemsConstant.ROLE_ID_PATIENT),
      this.clinicBranchesService.getAll()
    ]).subscribe(results => {
      this.medicalServices = results[0];
      this.patients = results[1];
      this.clinicBranches = results[2];

      if (this.entityId)
        this.loadAppointmentById(this.entityId);
    });
  }

  filterMedicalServices(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.medicalServices.length; i++) {
      let item = this.medicalServices[i];
      if (item.title.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredMedicalServices = filtered;
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

  filterClinicBranches(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.clinicBranches.length; i++) {
      let item = this.clinicBranches[i];
      if (item.name.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredClinicBranches = filtered;
  }

  saveData(): void {
    this.blockedPanel = true;

    let formData = this.entityForm.getRawValue();
    let requestedData: Appointment = {
      id: 0,
      clinicBranchName: '',
      createdDate: '',
      medicalServiceTitle: '',
      modifiedDate: '',
      patientFullName: '',
      statusCategoryId: this.statusCategoryId,
      statusCategoryName: '',
      appointmentDate: formData.appointmentDate,
      clinicBranchId: formData.clinicBranchId.id,
      medicalServiceId: formData.medicalServiceId.id,
      patientId: formData.patientId.id,
      note: formData.note,
      guessFullName: formData.guessFullName,
      guessPhoneNumber: formData.guessPhoneNumber
    };

    if (this.entityId) {
      this.subscriptions.push(this.appointmentsService.update(this.entityId, requestedData).subscribe(res => {
        this.ref.close();
        this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    }
    else {
      this.subscriptions.push(this.appointmentsService.add(requestedData).subscribe(res => {
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
