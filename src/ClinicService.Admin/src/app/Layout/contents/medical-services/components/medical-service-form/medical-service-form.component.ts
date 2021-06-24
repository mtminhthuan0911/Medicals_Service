import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesConstant } from '@app/shared/constants';
import { MedicalService, MedicalServiceType } from '@app/shared/models';
import { ErrorsService, MedicalServicesService, MedicalServiceTypesService, NotificationsService, UtilitiesService } from '@app/shared/services';
import { TinyMceService } from '@app/shared/services/tinymce.service';
import { environment } from '@environments/environment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-medical-service-form',
  templateUrl: './medical-service-form.component.html',
  styleUrls: ['./medical-service-form.component.css']
})
export class MedicalServiceFormComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  public blockedPanel = false;

  public entityId: number = this.activatedRoute.snapshot.params.id ? parseInt(this.activatedRoute.snapshot.params.id) : null;
  public entityForm: FormGroup;

  public attachments: any[] = [];
  public selectedFiles: File[] = [];

  public medicalServiceTypes: MedicalServiceType[] = [];
  public filteredMedicalServiceTypes: MedicalServiceType[];

  public serverPath = environment.apiUrl;

  // Define element's validation
  public VALIDATION_MESSAGES = {
    'title': [
      { type: 'required', message: MessagesConstant.REQUIRED },
      { type: 'maxlength', message: 'Không được nhập quá 256 ký tự.' }
    ],
    'medicalServiceTypeId': [
      { type: 'required', message: MessagesConstant.REQUIRED },
    ]
  };

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private medicalServicesService: MedicalServicesService,
    private medicalServiceTypesService: MedicalServiceTypesService,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private utilitiesService: UtilitiesService,
    private errorsService: ErrorsService,
    public tinyMceService: TinyMceService
  ) { }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      'title': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(256)
      ])),
      'description': new FormControl(''),
      'content': new FormControl(''),
      'cost': new FormControl(''),
      'medicalServiceTypeId': new FormControl('')
    });

    this.loadMedicalServiceTypes();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  loadMedicalServiceById(id: number): void {
    this.blockedPanel = true;

    this.subscriptions.push(this.medicalServicesService.getDetail(id).subscribe((res: MedicalService) => {
      this.entityForm.setValue({
        title: res.title,
        description: res.description,
        content: res.content,
        cost: res.cost,
        medicalServiceTypeId: this.medicalServiceTypes.find(f => f.id == res.medicalServiceTypeId)
      });

      this.attachments = res.attachments;

      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, errors => {
      this.errorsService.notifyErrors(errors);
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }));
  }

  loadMedicalServiceTypes() {
    this.blockedPanel = true;

    this.subscriptions.push(this.medicalServiceTypesService.getAll().subscribe(res => {
      this.medicalServiceTypes = res;
    },
      errors => {
        this.errorsService.notifyErrors(errors);
      },
      () => {
        if (this.entityId)
          this.loadMedicalServiceById(this.entityId);
        setTimeout(() => this.blockedPanel = false, 1000);
      }));
  }

  filterMedicalServiceTypes(event): void {
    let filtered: any[] = [];
    let query = event.query;
    for (let i = 0; i < this.medicalServiceTypes.length; i++) {
      let item = this.medicalServiceTypes[i];
      if (item.name.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(item);
      }
    }

    this.filteredMedicalServiceTypes = filtered;
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
    this.subscriptions.push(this.medicalServicesService.deleteAttachment(this.entityId, attachmentId)
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

    let rawForm = this.entityForm.getRawValue();
    let rawData: MedicalService = {
      title: rawForm.title,
      content: rawForm.content,
      description: rawForm.description,      
      cost: rawForm.cost,
      medicalServiceTypeId: rawForm.medicalServiceTypeId.id,
      id: null,
      attachments: null
    };

    const formData = this.utilitiesService.ToFormData(rawData);
    this.selectedFiles.forEach(file => formData.append('attachments', file, file.name));

    if (this.entityId) {
      this.subscriptions.push(this.medicalServicesService.update(this.entityId, formData).subscribe(() => {
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
      this.subscriptions.push(this.medicalServicesService.add(formData).subscribe(() => {
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
    this.router.navigateByUrl('/contents/medical-services');
  }
}
