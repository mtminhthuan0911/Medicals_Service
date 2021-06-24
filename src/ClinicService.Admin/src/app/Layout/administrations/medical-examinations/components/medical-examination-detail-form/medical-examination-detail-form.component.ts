import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { MedicalExaminationDetail } from '@app/shared/models';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-medical-examination-detail-form',
  templateUrl: './medical-examination-detail-form.component.html',
  styleUrls: ['./medical-examination-detail-form.component.css']
})
export class MedicalExaminationDetailFormComponent implements OnInit {
  public entityForm: FormGroup;

  public entityData: MedicalExaminationDetail = this.config.data ? this.config.data : null;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'diagnostic': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'treatment': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'diagnostic': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'treatment': new FormControl('', Validators.compose([
        Validators.required,
      ])),
    });

    if (this.entityData) {
      this.entityForm.setValue({
        diagnostic: this.entityData.diagnostic,
        treatment: this.entityData.treatment
      });
    }
  }

  saveData() {
    const submitData: MedicalExaminationDetail = this.entityForm.getRawValue();

    if (this.entityData) {
      this.entityData.diagnostic = submitData.diagnostic;
      this.entityData.treatment = submitData.treatment;
      this.ref.close(this.entityData);
    }
    else
      this.ref.close(submitData);
  }
}
