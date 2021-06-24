import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { Specialty } from '@app/shared/models';
import { ErrorsService, NotificationsService, SpecialtiesService, UtilitiesService } from '@app/shared/services';
import { ConfirmationService, TreeNode } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-specialties',
  templateUrl: './specialties.component.html',
  styleUrls: ['./specialties.component.css']
})
export class SpecialtiesComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  private ref: DynamicDialogRef;

  public blockedPanel: boolean = false;

  public items: TreeNode[];
  public selectedItem: Specialty = null;

  constructor(
    private router: Router,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private specialtiesService: SpecialtiesService,
    private utilitiesService: UtilitiesService,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // operate data methods
  loadData(): void {
    this.blockedPanel = true;

    this.subscription.add(this.specialtiesService.getAll().subscribe(res => {
      const specialtiesTree = this.utilitiesService.UnflatteringForTree(res);
      this.items = specialtiesTree;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false }, 1000);
      }));
  }

  // event methods
  onShowAddedModal(): void {
    this.router.navigateByUrl('/contents/specialty-form/');
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.router.navigateByUrl('/contents/specialty-form/' + this.selectedItem.id);
  }

  onConfirmDelete(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn xoá chuyên khoa này?',
      accept: () => {
        this.blockedPanel = true;

        const id = this.selectedItem.id;

        this.subscription.add(this.specialtiesService.delete(id).subscribe(() => {
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
}
