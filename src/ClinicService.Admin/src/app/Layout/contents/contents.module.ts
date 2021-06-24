import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ValidationMessagesModule } from '@app/shared';

import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { NotificationsService, ErrorsService } from '@app/shared/services';
import { FileUploadModule } from 'primeng/fileupload';
import { TreeTableModule } from 'primeng/treetable';

import { EditorModule } from '@tinymce/tinymce-angular';

import { SharedDirectivesModule } from '@app/shared/directives/shared-directives.module';

import { ContentsRoutingModule } from './contents-routing.module';

import { WebsiteSectionsComponent } from './website-sections/website-sections.component';
import { WebsiteSectionFormComponent } from './website-sections/components/website-section-form/website-section-form.component';
import { ClinicBranchesComponent } from './clinic-branches/clinic-branches.component';
import { ClinicBranchFormComponent } from './clinic-branches/components/clinic-branch-form/clinic-branch-form.component';
import { SpecialtiesComponent } from './specialties/specialties.component';
import { SpecialtyFormComponent } from './specialties/components/specialty-form/specialty-form.component';
import { MedicalServicesComponent } from './medical-services/medical-services.component';
import { MedicalServiceFormComponent } from './medical-services/components/medical-service-form/medical-service-form.component';
import { AutoCompleteModule } from 'primeng/autocomplete';

@NgModule({
  declarations: [
    WebsiteSectionsComponent,
    WebsiteSectionFormComponent,
    ClinicBranchesComponent,
    ClinicBranchFormComponent,
    SpecialtiesComponent,
    SpecialtyFormComponent,
    MedicalServicesComponent,
    MedicalServiceFormComponent
  ],
  imports: [
    CommonModule,
    ContentsRoutingModule,
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
    AutoCompleteModule
  ],
  providers: [
    DialogService,
    NotificationsService,
    ErrorsService
  ]
})
export class ContentsModule { }
