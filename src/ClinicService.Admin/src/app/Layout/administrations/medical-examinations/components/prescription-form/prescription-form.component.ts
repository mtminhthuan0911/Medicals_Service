import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessagesConstant } from '@app/shared/constants';
import { MedicalExaminationDetail, Prescription } from '@app/shared/models';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng';

@Component({
  selector: 'app-prescription-form',
  templateUrl: './prescription-form.component.html',
  styleUrls: ['./prescription-form.component.css']
})
export class PrescriptionFormComponent implements OnInit {
  public entityForm: FormGroup;

  public entityData = this.config.data ? this.config.data : null;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'subname': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'quantity': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'availableQuantity': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'total': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ],
    'take': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'name': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'subname': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'quantity': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'availableQuantity': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'total': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'take': new FormControl('', Validators.compose([
        Validators.required,
      ])),
      'isMorning': new FormControl(''),
      'isAfternoon': new FormControl(''),
      'isEvening': new FormControl(''),
      'note': new FormControl(''),
    });

    if (this.entityData) {
      console.log(this.entityData.subname);

      this.entityForm.setValue({
        name: this.entityData.name,
        subname: this.entityData.subname,
        quantity: this.entityData.quantity,
        availableQuantity: this.entityData.availableQuantity,
        total: this.entityData.total,
        take: this.entityData.take,
        isMorning: this.entityData.isMorning,
        isAfternoon: this.entityData.isAfternoon,
        isEvening: this.entityData.isEvening,
        note: this.entityData.note
      });
    }
  }

  saveData() {
    const submitData: Prescription = this.entityForm.getRawValue();

    if (this.entityData) {
      this.entityData.name = submitData.name;
      this.entityData.subname = submitData.subname;
      this.entityData.quantity = submitData.quantity;
      this.entityData.availableQuantity = submitData.availableQuantity;
      this.entityData.total = submitData.total;
      this.entityData.take = submitData.take;
      this.entityData.isMorning = submitData.isMorning;
      this.entityData.isAfternoon = submitData.isAfternoon;
      this.entityData.isEvening = submitData.isEvening;
      this.entityData.note = submitData.note;
      this.ref.close(this.entityData);
    }
    else
      this.ref.close(submitData);
  }
}
