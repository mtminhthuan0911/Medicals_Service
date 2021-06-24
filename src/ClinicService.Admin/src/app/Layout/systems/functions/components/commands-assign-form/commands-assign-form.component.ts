import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { FunctionsService, CommandsService, NotificationsService } from '@app/shared/services';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Command, CommandInFunction } from '@app/shared/models';
import { MessagesConstant } from '@app/shared/constants';

@Component({
  selector: 'app-commands-assign-form',
  templateUrl: './commands-assign-form.component.html',
  styleUrls: ['./commands-assign-form.component.css']
})
export class CommandsAssignFormComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public items: any[];
  public selectedItems = [];

  public entityId = this.config.data ? this.config.data.entityId : null;

  public blockedPanel = false;

  constructor(
    private functionsService: FunctionsService,
    private commandsService: CommandsService,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private notificationsService: NotificationsService,
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
    this.subscription.add(this.commandsService.getAll().subscribe((res: Command[]) => {
      this.items = res;
      this.loadCommandsOfFunction();
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  loadCommandsOfFunction(): void {
    const commands: Command[] = this.config.data ? this.config.data.commandsOfFunction : [];
    for (const cmd of commands) {
      const command = this.items.find(f => f.id === cmd.id);
      this.selectedItems.push(command);
    }
  }

  saveCommand(entity: CommandInFunction): void {
    this.blockedPanel = true;
    this.subscription.add(this.functionsService.postCommandByFunctionId(this.entityId, entity).subscribe(() => {
      this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.notificationsService.notifyError(errors);
      this.loadData();
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  deleteCommand(commandId: string): void {
    this.blockedPanel = true;
    this.subscription.add(this.functionsService.deleteCommandByFunctionId(this.entityId, commandId).subscribe(() => {
      this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.notificationsService.notifyError(errors);
        this.loadData();
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  // event methods
  onRowSelect(event: any): void {
    const commandInFunction: CommandInFunction = {
      commandId: event.data.id,
      functionId: this.entityId
    };

    this.saveCommand(commandInFunction);
  }

  onRowUnselect(event: any): void {
    this.deleteCommand(event.data.id);
  }
}
