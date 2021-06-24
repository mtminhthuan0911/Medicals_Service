import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, Reappointment } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ReappointmentsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<Reappointment[]>(`${environment.apiUrl}/api/re-appointments`, { headers: this._sharedHeader })
            .pipe(map((res: Reappointment[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<Reappointment>>(`${environment.apiUrl}/api/re-appointments/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<Reappointment>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: number) {
        return this.http.get<Reappointment>(`${environment.apiUrl}/api/re-appointments/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: Reappointment) {
        return this.http.post(`${environment.apiUrl}/api/re-appointments`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: number, entity: Reappointment) {
        return this.http.put(`${environment.apiUrl}/api/re-appointments/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    updateStatus(id: number, statusCategoryId: string) {
        return this.http.put(`${environment.apiUrl}/api/re-appointments/${id}/change-status/${statusCategoryId}`, null, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/re-appointments/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
