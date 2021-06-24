import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministrationsRoutingModule } from './administrations-routing.module';
import { ValidationMessagesModule } from '@app/shared';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedDirectivesModule } from '@app/shared/directives/shared-directives.module';
import { ErrorsService, NotificationsService } from '@app/shared/services';
import { AppointmentsComponent } from './appointments/appointments.component';
import { AppointmentFormComponent } from './appointments/components/appointment-form/appointment-form.component';

import { EditorModule } from '@tinymce/tinymce-angular';

import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { TreeTableModule } from 'primeng/treetable';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { AppointmentChangeStatusFormComponent } from './appointments/components/appointment-change-status-form/appointment-change-status-form.component';
import { TooltipModule } from 'primeng/tooltip';
import { MedicalExaminationsComponent } from './medical-examinations/medical-examinations.component';
import { MedicalExaminationFormComponent } from './medical-examinations/components/medical-examination-form/medical-examination-form.component';
import { MedicalExaminationDetailFormComponent } from './medical-examinations/components/medical-examination-detail-form/medical-examination-detail-form.component';
import { PrescriptionFormComponent } from './medical-examinations/components/prescription-form/prescription-form.component';
import { CheckboxModule } from 'primeng/checkbox';
import { ReappointmentsComponent } from './reappointments/reappointments.component';
import { ReappointmentFormComponent } from './reappointments/components/reappointment-form/reappointment-form.component';
import { ReappointmentChangeStatusFormComponent } from './reappointments/components/reappointment-change-status-form/reappointment-change-status-form.component';
import { MedicalExaminationChangeStatusFormComponent } from './medical-examinations/components/medical-examination-change-status-form/medical-examination-change-status-form.component';


@NgModule({
  declarations: [
    AppointmentsComponent,
    AppointmentFormComponent,
    AppointmentChangeStatusFormComponent,
    MedicalExaminationsComponent,
    MedicalExaminationFormComponent,
    MedicalExaminationDetailFormComponent,
    PrescriptionFormComponent,
    ReappointmentsComponent,
    ReappointmentFormComponent,
    ReappointmentChangeStatusFormComponent,
    MedicalExaminationChangeStatusFormComponent
  ],
  imports: [
    CommonModule,
    AdministrationsRoutingModule,
    ValidationMessagesModule,
    FormsModule,
    ReactiveFormsModule,
    PanelModule,
    ButtonModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ProgressSpinnerModule,
    DialogModule,
    InputTextModule,
    DynamicDialogModule,
    CalendarModule,
    DropdownModule,
    FileUploadModule,
    SharedDirectivesModule,
    TreeTableModule,
    EditorModule,
    AutoCompleteModule,
    InputTextareaModule,
    TooltipModule,
    CheckboxModule
  ],
  providers: [
    DialogService,
    NotificationsService,
    ErrorsService
  ]
})
export class AdministrationsModule { }
