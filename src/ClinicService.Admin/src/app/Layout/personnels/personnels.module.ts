import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PersonnelsRoutingModule } from './personnels-routing.module';
import { ValidationMessagesModule } from '@app/shared/modules';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { SharedDirectivesModule } from '@app/shared/directives/shared-directives.module';
import { TreeTableModule } from 'primeng/treetable';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PatientsComponent } from './patients/patients.component';
import { PatientDetailComponent } from './patients/components/patient-detail/patient-detail.component';
import { PatientMedicalExaminationDetailComponent } from './patients/components/patient-medical-examination-detail/patient-medical-examination-detail.component';
import { TooltipModule } from 'primeng/tooltip';


@NgModule({
  declarations: [
    PatientsComponent,
    PatientDetailComponent,
    PatientMedicalExaminationDetailComponent
  ],
  imports: [
    CommonModule,
    PersonnelsRoutingModule,
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
    TooltipModule,
    AutoCompleteModule
  ]
})
export class PersonnelsModule { }
