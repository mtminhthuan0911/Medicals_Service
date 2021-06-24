import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { Specialty } from '@app/shared/models';
import { ErrorsService, FileManagersService, NotificationsService, SpecialtiesService, UtilitiesService } from '@app/shared/services';
import { TinyMceService } from '@app/shared/services/tinymce.service';
import { environment } from '@environments/environment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-specialty-form',
  templateUrl: './specialty-form.component.html',
  styleUrls: ['./specialty-form.component.css']
})
export class SpecialtyFormComponent implements OnInit, OnDestroy {
  public blockedPanel = false;

  private subscriptions: Subscription[] = [];

  public entityId = this.activatedRoute.snapshot.params.id ? this.activatedRoute.snapshot.params.id : null;
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
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private errorsService: ErrorsService,
    private utilitiesService: UtilitiesService,
    private specialtiesService: SpecialtiesService,
    public tinyMceService: TinyMceService
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
      'description': new FormControl(''),
      'content': new FormControl(''),
      'sortOrder': new FormControl(''),
      'parentId': new FormControl(''),
      'seoAlias': new FormControl(''),
    });

    if (this.entityId) {
      this.entityForm.controls['id'].disable({ onlySelf: true });
      this.loadSpecialtyById(this.entityId);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadSpecialtyById(id: string) {
    this.blockedPanel = true;

    this.subscriptions.push(this.specialtiesService.getDetail(id).subscribe((res: Specialty) => {
      this.entityForm.setValue({
        id: res.id,
        name: res.name,
        description: res.description,
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
    this.subscriptions.push(this.specialtiesService.deleteAttachment(this.entityId, attachmentId)
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
      this.subscriptions.push(this.specialtiesService.update(this.entityId, formData).subscribe(() => {
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
      const entity: Specialty = this.entityForm.getRawValue();
      entity.sortOrder = parseInt(entity.sortOrder.toString(), 0);
      this.subscriptions.push(this.specialtiesService.add(formData).subscribe(() => {
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
    this.router.navigateByUrl('/contents/specialties');
  }
}
