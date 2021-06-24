import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { FunctionsComponent } from './functions/functions.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { AuthGuard } from '@app/shared';

const routes: Routes = [
    {
        path: '',
        component: UsersComponent,
        data: {
            functionCode: 'SYSTEM_USER'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'users',
        component: UsersComponent,
        data: {
            functionCode: 'SYSTEM_USER'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'roles',
        component: RolesComponent,
        data: {
            functionCode: 'SYSTEM_ROLE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'functions',
        component: FunctionsComponent,
        data: {
            functionCode: 'SYSTEM_FUNCTION'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'permissions',
        component: PermissionsComponent,
        data: {
            functionCode: 'SYSTEM_PERMISSION'
        },
        canActivate: [AuthGuard]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SystemsRoutingModule {}
