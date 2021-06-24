import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CategoriesRoutingModule } from './categories-routing.module';
import { StatusCategoriesComponent } from './status-categories/status-categories.component';
import { StatusCategoryFormComponent } from './status-categories/components/status-category-form/status-category-form.component';
import { ValidationMessagesModule } from '@app/shared';
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
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';
import { PaymentMethodsComponent } from './payment-methods/payment-methods.component';
import { PaymentMethodFormComponent } from './payment-methods/components/payment-method-form/payment-method-form.component';
import { MedicalServiceTypesComponent } from './medical-service-types/medical-service-types.component';
import { MedicalServiceTypeFormComponent } from './medical-service-types/components/medical-service-type-form/medical-service-type-form.component';


@NgModule({
  declarations: [
    StatusCategoriesComponent,
    StatusCategoryFormComponent,
    PaymentMethodsComponent,
    PaymentMethodFormComponent,
    MedicalServiceTypesComponent,
    MedicalServiceTypeFormComponent
  ],
  imports: [
    CommonModule,
    CategoriesRoutingModule,
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
  ]
})
export class CategoriesModule { }
