import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';

import { HeaderComponent } from './components/header/header.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { LayoutComponent } from './layout.component';

import { LayoutRoutingModule } from './layout-routing.module';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

import { ErrorsService, NotificationsService } from '@app/shared/services';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { SystemsModule } from './systems/systems.module';

@NgModule({
    imports: [
        CommonModule,
        LayoutRoutingModule,
        TranslateModule,
        NgbDropdownModule,
        ToastModule,
        ConfirmDialogModule,
        SystemsModule
    ],
    declarations: [
        LayoutComponent,
        SidebarComponent,
        HeaderComponent,
    ],
    providers: [
        MessageService,
        ConfirmationService,
        DialogService,
        NotificationsService,
        ErrorsService
    ]
})
export class LayoutModule { }
