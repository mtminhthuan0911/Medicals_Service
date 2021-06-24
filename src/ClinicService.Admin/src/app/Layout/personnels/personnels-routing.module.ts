import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@app/shared';
import { PatientDetailComponent } from './patients/components/patient-detail/patient-detail.component';
import { PatientsComponent } from './patients/patients.component';


const routes: Routes = [
  {
    path: 'patients',
        component: PatientsComponent,
        data: {
            functionCode: 'PERSONNEL_PATIENT'
        },
        canActivate: [AuthGuard]
  },
  {
    path: 'patients/detail/:id',
        component: PatientDetailComponent,
        data: {
            functionCode: 'PERSONNEL_PATIENT'
        },
        canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PersonnelsRoutingModule { }
