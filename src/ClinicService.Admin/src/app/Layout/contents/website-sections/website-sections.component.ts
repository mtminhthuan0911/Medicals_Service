import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { WebsiteSection } from '@app/shared/models';
import { ErrorsService, NotificationsService, UtilitiesService, WebsiteSectionsService } from '@app/shared/services';
import { ConfirmationService, TreeNode } from 'primeng/api';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-website-sections',
  templateUrl: './website-sections.component.html',
  styleUrls: ['./website-sections.component.css']
})
export class WebsiteSectionsComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  private ref: DynamicDialogRef;

  public blockedPanel: boolean = false;

  public items: TreeNode[];
  public selectedItem: WebsiteSection = null;

  constructor(
    private router: Router,
    private confirmationService: ConfirmationService,
    private websiteSectionService: WebsiteSectionsService,
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

    this.subscription.add(this.websiteSectionService.getAll().subscribe(res => {
      const webSectionTree = this.utilitiesService.UnflatteringForTree(res);
      this.items = webSectionTree;
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false }, 1000);
      }));
  }

  // event data method
  onShowAddedModal(): void {
    this.router.navigateByUrl('/contents/website-section-form/');
  }

  onShowEditedModal(): void {
    if (this.selectedItem === null) {
      this.notificationsService.notifyError(MessagesConstant.NOT_CHOOSE_ANY_RECORD);
      return;
    }

    this.router.navigateByUrl('/contents/website-section-form/' + this.selectedItem.id);
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

        this.subscription.add(this.websiteSectionService.delete(id).subscribe(() => {
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
