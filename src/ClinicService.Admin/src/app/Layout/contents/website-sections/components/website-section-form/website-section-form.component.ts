import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { WebsiteSection } from '@app/shared/models';
import { ErrorsService, NotificationsService, UtilitiesService, WebsiteSectionsService } from '@app/shared/services';
import { TinyMceService } from '@app/shared/services/tinymce.service';
import { environment } from '@environments/environment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-website-section-form',
  templateUrl: './website-section-form.component.html',
  styleUrls: ['./website-section-form.component.css']
})
export class WebsiteSectionFormComponent implements OnInit, OnDestroy {

  public blockedPanel = false;

  private subscriptions: Subscription[] = [];

  public entityId: string = this.activatedRoute.snapshot.params.id ? this.activatedRoute.snapshot.params.id : null;
  public entityForm: FormGroup;

  public attachments: any[] = [];
  public selectedFiles: File[] = [];

  public serverPath = environment.apiUrl;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'id': [
      { type: 'required', message: MessagesConstant.REQUIRED }
    ],
    'name': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ]
  };

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService,
    private utilitiesService: UtilitiesService,
    private websiteSectionsService: WebsiteSectionsService,
    public tinyMceService: TinyMceService,
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'id': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
      'content': new FormControl(''),
      'sortOrder': new FormControl(''),
      'parentId': new FormControl(''),
      'seoAlias': new FormControl(''),
    });

    if (this.entityId) {
      this.entityForm.controls['id'].disable({ onlySelf: true });
      this.loadWebsiteSectionById(this.entityId);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadWebsiteSectionById(id: string) {
    this.blockedPanel = true;

    this.subscriptions.push(this.websiteSectionsService.getDetail(id).subscribe((res: WebsiteSection) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        content: res.content,
        sortOrder: res.sortOrder,
        parentId: res.parentId,
        seoAlias: res.seoAlias
      });

      this.attachments = res.attachments;

      setTimeout(() => { this.blockedPanel = false; }, 1000);
    },
      errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  selectAttachments($event) {
    if ($event.currentFiles) {
      $event.currentFiles.forEach(file => {
        this.selectedFiles.push(file);
      });
    }
  }

  removeAttachments($event) {
    if ($event.file)
      this.selectedFiles.splice(this.selectedFiles.findIndex(item => item.name === $event.file.name), 1);
  }

  deleteAttachment(attachmentId) {
    this.blockedPanel = true;
    this.subscriptions.push(this.websiteSectionsService.deleteAttachment(this.entityId, attachmentId)
      .subscribe(() => {
        this.notificationsService.notifySuccess(MessagesConstant.DELETED_OK);
        this.attachments.splice(this.attachments.findIndex(item => item.id === attachmentId), 1);
        setTimeout(() => { this.blockedPanel = false; }, 1000);

      }, errors => {
        this.errorsService.notifyErrors(errors);
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
    return false;
  }

  saveData(): void {
    this.blockedPanel = true;

    const formData = this.utilitiesService.ToFormData(this.entityForm.getRawValue());
    this.selectedFiles.forEach(file => formData.append('attachments', file, file.name));

    if (this.entityId) {
      this.subscriptions.push(this.websiteSectionsService.update(this.entityId, formData).subscribe(() => {
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
        () => {
          this.notificationsService.notifySuccess(MessagesConstant.UPDATED_OK);
          this.goBack();
        }));
    } else {
      this.subscriptions.push(this.websiteSectionsService.add(formData).subscribe(() => {
      },
        errors => {
          this.errorsService.notifyErrors(errors);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        },
        () => {
          this.notificationsService.notifySuccess(MessagesConstant.CREATED_OK);
          this.goBack();
        }));
    }
  }

  goBack(): void {
    this.router.navigateByUrl('/contents/website-sections');
  }
}
