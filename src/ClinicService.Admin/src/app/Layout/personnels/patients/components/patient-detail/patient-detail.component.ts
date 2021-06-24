import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicalExaminationsService, UsersService } from '@app/shared/services';
import { User } from 'oidc-client';
import { DialogService } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';
import { PatientMedicalExaminationDetailComponent } from '../patient-medical-examination-detail/patient-medical-examination-detail.component';

@Component({
  selector: 'app-patient-detail',
  templateUrl: './patient-detail.component.html',
  styleUrls: ['./patient-detail.component.css']
})
export class PatientDetailComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  private patientId = this.activatedRoute.snapshot.params.id ? this.activatedRoute.snapshot.params.id : null;

  public patient: any;
  public medicalExaminations: any[];

  constructor(
    private usersService: UsersService,
    private medicalExaminationsService: MedicalExaminationsService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.loadPatientInfo();
    this.loadMedicalExaminationsByPatientId();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadPatientInfo() {
    if (this.patientId)
      this.subscription.add(this.usersService.getDetail(this.patientId).subscribe(res => {
        this.patient = res;
      }));
  }

  loadMedicalExaminationsByPatientId() {
    if (this.patientId)
      this.subscription.add(this.medicalExaminationsService.getAllByPatientId(this.patientId).subscribe(res => {
        this.medicalExaminations = res;
      }));
  }

  // event methods
  goBack() {
    this.router.navigate(['personnels/patients']);
  }

  goToPatientMedicalExaminatilDetail(medicalExaminationId: number) {
    this.dialogService.open(PatientMedicalExaminationDetailComponent, {
      header: 'Thông tin kết quả khám',
      data: { entityId: medicalExaminationId },
      width: '70%',
    });
  }
}
