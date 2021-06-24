import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { RolesService, PermissionsService, NotificationsService, UtilitiesService, ErrorsService } from '@app/shared/services';
import { Role, PermissionScreen, Permission } from '@app/shared/models';
import { CommandsConstant, MessagesConstant } from '@app/shared/constants';
import { ConfirmationService } from 'primeng/api';
import { PermissionUpdateRequest } from '@app/shared/models/permission-update-request.model';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.css']
})
export class PermissionsComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public blockedPanel = false;

  private flattenFunctions: any[] = [];

  public roleOptions = [];
  public selectedRole: Role = null;

  public permissionScreen = [];

  public selectedViews = [];
  public selectedCreates = [];
  public selectedUpdates = [];
  public selectedDeletes = [];
  public selectedApproves = [];

  constructor(
    private rolesService: RolesService,
    private permissionsService: PermissionsService,
    private notificationsService: NotificationsService,
    private utilitiesService: UtilitiesService,
    private errorsService: ErrorsService,
    private confirmationService: ConfirmationService,
  ) { }

  ngOnInit(): void {
    this.loadRoleOptions();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  loadRoleOptions(): void {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getAll().subscribe((res: Role[]) => {
      this.roleOptions = res;
      this.selectedRole = this.roleOptions[0];
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      },
      () => {
        this.loadData(this.selectedRole.id);
      }));
  }

  loadData(roleId: string): void {
    this.blockedPanel = true;
    this.subscription.add(this.permissionsService.getCommandsView().subscribe((res: PermissionScreen[]) => {
      this.permissionScreen = this.utilitiesService.UnflatteringForTree(res);
      this.flattenFunctions = res;
      this.loadPermissionsByRoleId(roleId);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  loadPermissionsByRoleId(roleId: string): void {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getPermissionByRoleId(roleId).subscribe((res: Permission[]) => {
      // Reset selected commands
      this.selectedViews = [];
      this.selectedCreates = [];
      this.selectedUpdates = [];
      this.selectedDeletes = [];
      this.selectedApproves = [];

      // push functionId to selected command
      res.forEach(element => {
        if (element.commandId === CommandsConstant.READ_ACTION) {
          this.selectedViews.push(element.functionId);
        }
        if (element.commandId === CommandsConstant.CREATE_ACTION) {
          this.selectedCreates.push(element.functionId);
        }
        if (element.commandId === CommandsConstant.UPDATE_ACTION) {
          this.selectedUpdates.push(element.functionId);
        }
        if (element.commandId === CommandsConstant.DELETE_ACTION) {
          this.selectedDeletes.push(element.functionId);
        }
      });
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  selectedCommands(checked: boolean, functionId: string, parentId: string, functionOfCommands: any[]): any[] {
    functionOfCommands = functionOfCommands.filter(f => f !== functionId);
    if (checked) {
      functionOfCommands.push(functionId);
      if (parentId === undefined || parentId === null) {
        // if item is parent, select children of this item.
        const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(m => m.id);
        functionOfCommands.push(...childFunctions);
      } else {
        // select parent when a child is selected
        if (functionOfCommands.filter(f => f === parentId).length === 0) {
          functionOfCommands.push(parentId);
        }
      }
    } else {
      // if item is only parent, just removed it.
      if (functionOfCommands.filter(f => f.includes(parentId)).length === 1) {
        functionOfCommands = functionOfCommands.filter(f => f !== parentId);
      }
      // removing children of parent if parent item is unselected.
      if (parentId === undefined || parentId === null) {
        const childFunctions = this.flattenFunctions.filter(f => f.parentId === functionId).map(m => m.id);
        functionOfCommands = functionOfCommands.filter(function (el) {
          return !childFunctions.includes(el);
        });
      }
    }

    return functionOfCommands;
  }

  savePermissions(): PermissionUpdateRequest {
    const updateRequest = new PermissionUpdateRequest();
    updateRequest.permissionViewModels = [];
    // push from view commands;
    if (this.selectedViews.length > 0) {
      this.selectedViews.forEach(element => {
        const permission = new Permission();
        permission.functionId = element;
        permission.roleId = this.selectedRole.id;
        permission.commandId = CommandsConstant.READ_ACTION;
        updateRequest.permissionViewModels.push(permission);
      });
    }

    // push from create commands;
    if (this.selectedCreates.length > 0) {
      this.selectedCreates.forEach(element => {
        const permission = new Permission();
        permission.functionId = element;
        permission.roleId = this.selectedRole.id;
        permission.commandId = CommandsConstant.CREATE_ACTION;
        updateRequest.permissionViewModels.push(permission);
      });
    }

    // push from update commands;
    if (this.selectedUpdates.length > 0) {
      this.selectedUpdates.forEach(element => {
        const permission = new Permission();
        permission.functionId = element;
        permission.roleId = this.selectedRole.id;
        permission.commandId = CommandsConstant.UPDATE_ACTION;
        updateRequest.permissionViewModels.push(permission);
      });
    }

    // push from delete commands;
    if (this.selectedDeletes.length > 0) {
      this.selectedDeletes.forEach(element => {
        const permission = new Permission();
        permission.functionId = element;
        permission.roleId = this.selectedRole.id;
        permission.commandId = CommandsConstant.DELETE_ACTION;
        updateRequest.permissionViewModels.push(permission);
      });
    }

    return updateRequest;
  }

  // event methods
  onChangeRole(event): void {
    this.loadData(event.value.id);
  }

  onChangeCommand(event: any, commandId: string, functionId: string, parentId: string) {
    if (commandId === CommandsConstant.READ_ACTION) {
      // CHECKED COMMANDS FOR VIEWS
      const functionOfCommands = this.selectedViews.filter(f => f !== functionId);
      this.selectedViews = this.selectedCommands(event.checked, functionId, parentId, functionOfCommands);
    } else if (commandId === CommandsConstant.CREATE_ACTION) {
      // CHECKED COMMANDS FOR CREATES
      const functionOfCommands = this.selectedCreates.filter(f => f !== functionId);
      this.selectedCreates = this.selectedCommands(event.checked, functionId, parentId, functionOfCommands);
    } else if (commandId === CommandsConstant.UPDATE_ACTION) {
      // CHECKED COMMANDS FOR UPDATES
      const functionOfCommands = this.selectedUpdates.filter(f => f !== functionId);
      this.selectedUpdates = this.selectedCommands(event.checked, functionId, parentId, functionOfCommands);
    } else if (commandId === CommandsConstant.DELETE_ACTION) {
      // CHECKED COMMANDS FOR DELETES
      const functionOfCommands = this.selectedDeletes.filter(f => f !== functionId);
      this.selectedDeletes = this.selectedCommands(event.checked, functionId, parentId, functionOfCommands);
    }
  }

  onSavePermissions(): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn lưu những thay đổi này?',
      accept: () => {
        this.blockedPanel = true;
        const updateRequest: PermissionUpdateRequest = this.savePermissions();
        this.subscription.add(this.rolesService.putPermissionByRoleId(this.selectedRole.id, updateRequest).subscribe(() => {
          this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
          errors => {
            this.errorsService.notifyErrors(errors);
            setTimeout(() => { this.blockedPanel = false; }, 1000);
          }));
      }
    });
  }
}
