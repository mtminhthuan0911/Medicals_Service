import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch, Specialty } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class SpecialtiesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<Specialty[]>(`${environment.apiUrl}/api/specialties`, { headers: this._sharedHeader })
            .pipe(map((res: Specialty[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<Specialty>>(`${environment.apiUrl}/api/specialties/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<Specialty>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<Specialty>(`${environment.apiUrl}/api/specialties/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: FormData) {
        return this.http.post(`${environment.apiUrl}/api/specialties`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: FormData) {
        return this.http.put(`${environment.apiUrl}/api/specialties/${id}`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/specialties/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteAttachment(specialtyId: string, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/specialties/${specialtyId}/attachments/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
