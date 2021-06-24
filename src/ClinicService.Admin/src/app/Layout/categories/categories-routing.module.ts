import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@app/shared';
import { MedicalServiceTypesComponent } from './medical-service-types/medical-service-types.component';
import { PaymentMethodsComponent } from './payment-methods/payment-methods.component';
import { StatusCategoriesComponent } from './status-categories/status-categories.component';


const routes: Routes = [
  {
    path: 'status-categories',
    component: StatusCategoriesComponent,
    data: {
      functionCode: 'CATEGORY_STATUS_CATEGORY'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'payment-methods',
    component: PaymentMethodsComponent,
    data: {
      functionCode: 'CATEGORY_PAYMENT_METHOD'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'medical-service-types',
    component: MedicalServiceTypesComponent,
    data: {
      functionCode: 'CATEGORY_MEDICAL_SERVICE_TYPE'
    },
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CategoriesRoutingModule { }
