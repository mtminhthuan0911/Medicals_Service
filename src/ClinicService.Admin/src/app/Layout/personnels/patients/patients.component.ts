import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SystemsConstant } from '@app/shared/constants';
import { Pagination, User } from '@app/shared/models';
import { ErrorsService, NotificationsService, UsersService } from '@app/shared/services';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html',
  styleUrls: ['./patients.component.css']
})
export class PatientsComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();

  // Default
  public blockedPanel = false;

  /**
     * Paging
     */
  public page = 1;
  public limit = 10;
  public totalRecords: number;
  public q = '';

  public items: any[];
  public selectedItem: User = null;

  constructor(
    private usersService: UsersService,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // operate data methods
  loadData() {
    this.blockedPanel = true;
    this.subscription.add(this.usersService.getAllByRoleIdSearch(SystemsConstant.ROLE_ID_PATIENT, this.q, this.page, this.limit)
      .subscribe((res: Pagination<User>) => {
        this.processLoadData(res);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
  
  processLoadData(res: Pagination<User>): void {
    this.items = res.items;
    this.page = this.page;
    this.limit = this.limit;
    this.totalRecords = res.totalRecords;
  }

  // event methods
  goToPatientDetail(id: string) {
    this.router.navigate(['personnels/patients/detail', id]);
  }
}
