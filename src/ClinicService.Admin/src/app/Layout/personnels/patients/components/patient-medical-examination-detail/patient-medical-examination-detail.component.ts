import { Component, OnDestroy, OnInit } from '@angular/core';
import { MedicalExaminationsService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-patient-medical-examination-detail',
  templateUrl: './patient-medical-examination-detail.component.html',
  styleUrls: ['./patient-medical-examination-detail.component.css']
})
export class PatientMedicalExaminationDetailComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  private entityId = this.config.data ? +this.config.data.entityId : null;

  public item: any;

  constructor(
    private medicalExaminationsService: MedicalExaminationsService,
    private ref: DynamicDialogRef, 
    private config: DynamicDialogConfig
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // operate data methods
  loadData() {
    if (this.entityId) {
      this.subscription.add(this.medicalExaminationsService.getDetail(this.entityId).subscribe(res => {
        this.item = res;
      }));
    }
  }
}
