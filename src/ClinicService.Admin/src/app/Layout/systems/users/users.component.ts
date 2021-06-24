import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { User, Pagination } from '@app/shared/models';
import { UsersService } from '@app/shared/services/users.service';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { NotificationsService, ErrorsService, AuthService } from '@app/shared/services';
import { ConfirmationService } from 'primeng/api';
import { MessagesConstant } from '@app/shared/constants';
import { UserFormComponent } from './components/user-form/user-form.component';
import { RolesAssignFormComponent } from './components/roles-assign-form/roles-assign-form.component';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();
  public ref: DynamicDialogRef;

  // Default
  public blockedPanel = false;

  /**
   * Paging
   */
  public page = 1;
  public limit = 10;
  public totalRecords: number;
  public q = '';

  // User
  public items: any[];
  public selectedItem: User = null;
  public rolesOfUser = [];

  // Roles
  constructor(
    private usersService: UsersService,
    private dialogService: DialogService,
    private notificationsService: NotificationsService,
    private confirmationService: ConfirmationService,
    private errorsService: ErrorsService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // operate data methods
  loadData(selectedId = null): void {
    this.blockedPanel = true;
    this.subscription.add(this.usersService.getSearch(this.q, this.page, this.limit).subscribe((res: Pagination<User>) => {
      this.processLoadData(selectedId, res);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  processLoadData(selectedId = null, res: Pagination<User>): void {
    this.items = res.items;
    this.page = this.page;
    this.limit = this.limit;
    this.totalRecords = res.totalRecords;

    if (this.selectedItem) {
      this.loadRoleNamesOfUser(this.selectedItem.id);
    }
  }

  loadRoleNamesOfUser(userId: string): void {
    this.subscription.add(this.usersService.getRolesByUserId(userId).subscribe((res: string[]) => {
      this.rolesOfUser = res;
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        this.rolesOfUser = [];
      }));
  }

  // event methods
  onPageChanged(event: any): void {
    this.page = event.page + 1;
    this.limit = event.rows;
    this.loadData();
  }

  onShowAddedModal(): void {
    this.ref = this.dialogService.open(UserFormComponent, {
      header: 'Thêm mới người dùng',
      width: '64%',
      height: '64%'
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = { entityId: this.selectedItem.id };

    this.ref = this.dialogService.open(UserFormComponent, {
      header: 'Cập nhật người dùng',
      data: data,
      width: '64%',
      height: '64%'
    });

    this.ref.onClose.subscribe(() => {
      this.loadData();
      this.selectedItem = null;
    });
  }

  onConfirmDelete(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá người dùng này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.usersService.delete(id).subscribe(() => {
          this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
          this.loadData();
          this.selectedItem = null;
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
          errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => { this.blockedPanel = false; }, 1000);
          }));
      }
    });
  }

  onShowRolesAssignModal(userId: string): void {
    if (this.selectedItem === null || this.selectedItem.id !== userId) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = {
      entityId: userId,
      roleNamesOfUser: this.rolesOfUser
    };

    this.ref = this.dialogService.open(RolesAssignFormComponent, {
      data: data,
      header: 'Xét quyền người dùng',
      width: '64%'
    });

    this.ref.onClose.subscribe(() => {
      this.loadRoleNamesOfUser(this.selectedItem.id);
    });
  }

  onConfirmResetPassword(id: string): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn reset mật khẩu cho thành viên này?',
      accept: () => {
        this.blockedPanel = true;

        this.subscription.add(this.usersService.resetPassword(id).subscribe(() => {
          this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
          this.loadData();
          this.selectedItem = null;
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
          errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => { this.blockedPanel = false; }, 1000);
          }));
      }
    });
  }


  onRowSelect(event: any): void {
    this.rolesOfUser = [];
    this.loadRoleNamesOfUser(event.data.id);
  }
}
