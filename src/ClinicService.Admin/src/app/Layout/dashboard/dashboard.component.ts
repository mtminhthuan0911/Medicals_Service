import { Component, OnInit, OnDestroy } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { Subscription } from 'rxjs';

import {
  UsersService,
  ErrorsService,
  AuthService,
  RolesService
} from '@app/shared/services';

import {
  Pagination,
} from '@app/shared/models';

import { LendingBaseStatusConstant } from '@app/shared/constants';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  animations: [routerTransition()]
})
export class DashboardComponent implements OnInit {
  
  constructor() { }

  ngOnInit(): void { }

}
