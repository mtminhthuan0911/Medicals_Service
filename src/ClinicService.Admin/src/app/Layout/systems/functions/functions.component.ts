import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { FunctionsService, NotificationsService, UtilitiesService, ErrorsService } from '@app/shared/services';
import { TreeNode, ConfirmationService } from 'primeng/api';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { FunctionFormComponent } from './components/function-form/function-form.component';
import { MessagesConstant } from '@app/shared/constants';
import { Function, Command } from '@app/shared/models';
import { CommandsAssignFormComponent } from './components/commands-assign-form/commands-assign-form.component';

@Component({
  selector: 'app-functions',
  templateUrl: './functions.component.html',
  styleUrls: ['./functions.component.css']
})
export class FunctionsComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();
  public ref: DynamicDialogRef;

  public blockedPanel = false;

  public items: TreeNode[];
  public selectedItem: Function = null;
  public commandsOfFunction = [];

  constructor(
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private functionsService: FunctionsService,
    private notificationsService: NotificationsService,
    private utilitiesService: UtilitiesService,
    private errorsService: ErrorsService
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

    this.subscription.add(this.functionsService.getAll().subscribe(res => {
      const functionTree = this.utilitiesService.UnflatteringForTree(res);
      this.items = functionTree;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  loadCommandsOfFunction(functionId: string): void {
    this.subscription.add(this.functionsService.getCommandsByFunctionId(functionId).subscribe((res: Command[]) => {
      this.commandsOfFunction = res;
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        this.commandsOfFunction = [];
      }));
  }

  // event data method
  onShowAddedModal(): void {
    this.ref = this.dialogService.open(FunctionFormComponent, {
      header: 'Thêm mới chức năng',
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

    const data = {
      entityId: this.selectedItem.id
    };

    this.ref = this.dialogService.open(FunctionFormComponent, {
      data: data,
      header: 'Cập nhật chức năng',
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
      message: 'Bạn có chắc chắn xoá chức năng này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.functionsService.delete(id).subscribe(() => {
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

  onShowCommandsAssignModal(functionId: string): void {
    if (this.selectedItem === null || this.selectedItem.id !== functionId) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    const data = {
      entityId: functionId,
      commandsOfFunction: this.commandsOfFunction
    };

    this.ref = this.dialogService.open(CommandsAssignFormComponent, {
      data: data,
      header: 'Gán lệnh chức năng',
      width: '64%'
    });

    this.ref.onClose.subscribe(() => {
      this.loadCommandsOfFunction(this.selectedItem.id);
    });
  }

  onNodeSelect(event: any): void {
    this.commandsOfFunction = [];
    this.loadCommandsOfFunction(event.node.id);
  }
}
