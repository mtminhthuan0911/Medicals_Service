import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Role, Pagination, PermissionUpdateRequest, StatusCategory } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class StatusCategoriesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<StatusCategory[]>(`${environment.apiUrl}/api/status-categories`, { headers: this._sharedHeader })
            .pipe(map((res: StatusCategory[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<StatusCategory>>(`${environment.apiUrl}/api/status-categories/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<StatusCategory>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<StatusCategory>(`${environment.apiUrl}/api/status-categories/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: StatusCategory) {
        return this.http.post(`${environment.apiUrl}/api/status-categories`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: StatusCategory) {
        return this.http.put(`${environment.apiUrl}/api/status-categories/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/status-categories/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
