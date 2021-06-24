import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { UsersService, NotificationsService, RolesService, ErrorsService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Role } from '@app/shared/models';
import { MessagesConstant } from '@app/shared/constants';

@Component({
  selector: 'app-roles-assign-form',
  templateUrl: './roles-assign-form.component.html',
  styleUrls: ['./roles-assign-form.component.css']
})
export class RolesAssignFormComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public items: any[];
  public selectedItems = [];

  public entityId = this.config.data ? this.config.data.entityId : null;

  public blockedPanel = false;

  constructor(
    private usersService: UsersService,
    private rolesService: RolesService,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.ref.close();
    this.subscription.unsubscribe();
  }

  // operate data methods
  loadData(): void {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getAll().subscribe((res: Role[]) => {
      this.items = res;
      this.loadRolesOfUser();
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  loadRolesOfUser(): void {
    const roleNames = this.config.data ? this.config.data.roleNamesOfUser : [];
    for (const roleName of roleNames) {
      const role = this.items.find(f => f.name === roleName);
      this.selectedItems.push(role);
    }
  }

  saveRole(roleName: string): void {
    this.blockedPanel = true;
    this.subscription.add(this.usersService.postRoleByUserId(this.entityId, roleName).subscribe(() => {
      this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        this.loadData();
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  deleteRole(roleName: string): void {
    this.blockedPanel = true;
    this.subscription.add(this.usersService.deleteRoleByUserId(this.entityId, roleName).subscribe(() => {
      this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        this.loadData();
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  // event methods
  onRowSelect(event: any): void {
    const roleName = event.data.name;
    this.saveRole(roleName);
  }

  onRowUnselect(event: any): void {
    const roleName = event.data.name;
    this.deleteRole(roleName);
  }
}
