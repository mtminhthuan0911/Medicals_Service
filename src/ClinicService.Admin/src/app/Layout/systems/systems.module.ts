import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { FunctionsComponent } from './functions/functions.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { SystemsRoutingModule } from './systems-routing.module';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DynamicDialogModule, DialogService } from 'primeng/dynamicdialog';
import { CalendarModule } from 'primeng/calendar';
import { RoleFormComponent } from './roles/components/role-form/role-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NotificationsService, ErrorsService } from '@app/shared/services';
import { ValidationMessagesModule } from '@app/shared/modules';
import { UserFormComponent } from './users/components/user-form/user-form.component';
import { RolesAssignFormComponent } from './users/components/roles-assign-form/roles-assign-form.component';
import { TreeTableModule } from 'primeng/treetable';
import { FunctionFormComponent } from './functions/components/function-form/function-form.component';
import { CommandsAssignFormComponent } from './functions/components/commands-assign-form/commands-assign-form.component';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { TooltipModule } from 'primeng/tooltip';
import { SharedDirectivesModule } from '@app/shared/directives/shared-directives.module';
import { ChangePasswordFormComponent } from './users/components/change-password-form/change-password-form.component';

@NgModule({
  declarations: [
    UsersComponent,
    RolesComponent,
    FunctionsComponent,
    PermissionsComponent,
    RoleFormComponent,
    UserFormComponent,
    RolesAssignFormComponent,
    FunctionFormComponent,
    CommandsAssignFormComponent,
    ChangePasswordFormComponent
  ],
  imports: [
    CommonModule,
    SystemsRoutingModule,
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
    TreeTableModule,
    DropdownModule,
    CheckboxModule,
    TooltipModule,
    SharedDirectivesModule
  ],
  providers: [
    DialogService,
    NotificationsService,
    ErrorsService
  ]
})
export class SystemsModule { }
