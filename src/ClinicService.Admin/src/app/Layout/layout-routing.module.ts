import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './layout.component';
import { AuthGuard } from '@app/shared';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            { path: '', redirectTo: 'dashboard', pathMatch: 'prefix' },
            {
                path: 'dashboard',
                loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
                data: {
                    functionCode: 'DASHBOARD'
                },
                canActivate: [AuthGuard]
            },
            {
                path: 'contents',
                loadChildren: () => import('./contents/contents.module').then((m) => m.ContentsModule),
                data: {
                    functionCode: 'CONTENT'
                },
                canActivate: [AuthGuard]
            },
            {
                path: 'administrations',
                loadChildren: () => import('./administrations/administrations.module').then((m) => m.AdministrationsModule),
                data: {
                    functionCode: 'ADMINISTRATION'
                },
                canActivate: [AuthGuard]
            },
            {
                path: 'categories',
                loadChildren: () => import('./categories/categories.module').then((m) => m.CategoriesModule),
                data: {
                    functionCode: 'CATEGORY'
                },
                canActivate: [AuthGuard]
            },
            {
                path: 'personnels',
                loadChildren: () => import('./personnels/personnels.module').then((m) => m.PersonnelsModule),
                data: {
                    functionCode: 'PERSONNEL'
                },
                canActivate: [AuthGuard]
            },
            {
                path: 'systems',
                loadChildren: () => import('./systems/systems.module').then((m) => m.SystemsModule),
                data: {
                    functionCode: 'SYSTEM'
                },
                canActivate: [AuthGuard]
            },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LayoutRoutingModule {}
