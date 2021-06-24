import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/shared';

import { ClinicBranchesComponent } from './clinic-branches/clinic-branches.component';
import { MedicalServiceFormComponent } from './medical-services/components/medical-service-form/medical-service-form.component';
import { MedicalServicesComponent } from './medical-services/medical-services.component';
import { SpecialtyFormComponent } from './specialties/components/specialty-form/specialty-form.component';
import { SpecialtiesComponent } from './specialties/specialties.component';
import { WebsiteSectionFormComponent } from './website-sections/components/website-section-form/website-section-form.component';
import { WebsiteSectionsComponent } from './website-sections/website-sections.component';

const routes: Routes = [
    {
        path: 'website-sections',
        component: WebsiteSectionsComponent,
        data: {
            functionCode: 'CONTENT_WEBSITE_SECTION'
        },
        canActivate: [AuthGuard]
    },
    { 
        path: 'website-section-form',
        component: WebsiteSectionFormComponent,
        data: {
            functionCode: 'CONTENT_WEBSITE_SECTION'
        },
        canActivate: [AuthGuard]
    },
    { 
        path: 'website-section-form/:id',
        component: WebsiteSectionFormComponent,
        data: {
            functionCode: 'CONTENT_WEBSITE_SECTION'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'clinic-branches',
        component: ClinicBranchesComponent,
        data: {
            functionCode: 'CONTENT_CLINIC_BRANCH'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'specialties',
        component: SpecialtiesComponent,
        data: {
            functionCode: 'CONTENT_SPECIALTY'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'specialty-form',
        component: SpecialtyFormComponent,
        data: {
            functionCode: 'CONTENT_SPECIALTY'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'specialty-form/:id',
        component: SpecialtyFormComponent,
        data: {
            functionCode: 'CONTENT_SPECIALTY'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'medical-services',
        component: MedicalServicesComponent,
        data: {
            functionCode: 'CONTENT_MEDICAL_SERVICE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'medical-service-form',
        component: MedicalServiceFormComponent,
        data: {
            functionCode: 'CONTENT_MEDICAL_SERVICE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'medical-service-form/:id',
        component: MedicalServiceFormComponent,
        data: {
            functionCode: 'CONTENT_MEDICAL_SERVICE'
        },
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContentsRoutingModule { }
