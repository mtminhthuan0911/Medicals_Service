import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TinyMceService extends BaseService {
    public Configuration: any = {
        height: 500,
        menubar: false,
        images_upload_url: 'https://localhost:5001/api/file-managers/upload-from-editor',

        plugins: [
            'advlist autolink lists link image charmap print preview anchor',
            'searchreplace visualblocks code fullscreen image',
            'insertdatetime media table paste code help wordcount',
            'textcolor'
        ],
        toolbar:
            'undo redo | formatselect | bold italic forecolor backcolor | \
          alignleft aligncenter alignright alignjustify | image | \
          bullist numlist outdent indent | removeformat | help'
    };

    public ApiKey: string = 'fvhiga6g5jn81zx6rwjsb3017xhirt9zztngfz6toa5px4wg';

    constructor() {
        super();
    }


}
