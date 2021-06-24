import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Pagination, WebsiteSection } from '../models';

@Injectable({ providedIn: 'root' })
export class WebsiteSectionsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<WebsiteSection[]>(`${environment.apiUrl}/api/website-sections`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<WebsiteSection>(`${environment.apiUrl}/api/website-sections/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: FormData) {
        return this.http.post(`${environment.apiUrl}/api/website-sections`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: FormData) {
        return this.http.put(`${environment.apiUrl}/api/website-sections/${id}`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/website-sections/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteAttachment(websiteSectionId: string, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/website-sections/${websiteSectionId}/attachments/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
