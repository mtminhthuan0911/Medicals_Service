import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';
import { MedicalService } from '../models/medical-service.model';

@Injectable({ providedIn: 'root' })
export class MedicalServicesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<MedicalService[]>(`${environment.apiUrl}/api/medical-services`, { headers: this._sharedHeader })
            .pipe(map((res: MedicalService[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<MedicalService>>(`${environment.apiUrl}/api/medical-services/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<MedicalService>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: number) {
        return this.http.get<MedicalService>(`${environment.apiUrl}/api/medical-services/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: FormData) {
        return this.http.post(`${environment.apiUrl}/api/medical-services`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    update(id: number, entity: FormData) {
        return this.http.put(`${environment.apiUrl}/api/medical-services/${id}`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    delete(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/medical-services/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteAttachment(medicalServiceId: number, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/website-sections/${medicalServiceId}/attachments/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
