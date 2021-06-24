import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@app/shared';
import { AppointmentsComponent } from './appointments/appointments.component';
import { MedicalExaminationFormComponent } from './medical-examinations/components/medical-examination-form/medical-examination-form.component';
import { MedicalExaminationsComponent } from './medical-examinations/medical-examinations.component';
import { ReappointmentsComponent } from './reappointments/reappointments.component';

const routes: Routes = [
  {
    path: 'appointments',
    component: AppointmentsComponent,
    data: {
      functionCode: 'ADMINISTRATION_APPOINTMENT'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-examinations',
    component: MedicalExaminationsComponent,
    data: {
      functionCode: 'ADMINISTRATION_MEDICAL_EXAMINATION'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-examination-form',
    component: MedicalExaminationFormComponent,
    data: {
      functionCode: 'ADMINISTRATION_MEDICAL_EXAMINATION'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-examination-form/from-appointment/:appointmentId/:patientId',
    component: MedicalExaminationFormComponent,
    data: {
      functionCode: 'ADMINISTRATION_MEDICAL_EXAMINATION'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-examination-form/from-reappointment/:reappointmentId/:patientId',
    component: MedicalExaminationFormComponent,
    data: {
      functionCode: 'ADMINISTRATION_MEDICAL_EXAMINATION'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-examination-form/:id',
    component: MedicalExaminationFormComponent,
    data: {
      functionCode: 'ADMINISTRATION_MEDICAL_EXAMINATION'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 're-appointments',
    component: ReappointmentsComponent,
    data: {
      functionCode: 'ADMINISTRATION_REAPPOINTMENT'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 're-appointments/from-medical-examination/:fromMedicalExaminationId',
    component: ReappointmentsComponent,
    data: {
      functionCode: 'ADMINISTRATION_REAPPOINTMENT'
    },
    canActivate: [AuthGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdministrationsRoutingModule { }
