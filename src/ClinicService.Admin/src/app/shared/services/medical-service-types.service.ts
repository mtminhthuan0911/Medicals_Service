import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, MedicalServiceType } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class MedicalServiceTypesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<MedicalServiceType[]>(`${environment.apiUrl}/api/medical-service-types`, { headers: this._sharedHeader })
            .pipe(map((res: MedicalServiceType[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<MedicalServiceType>>(`${environment.apiUrl}/api/medical-service-types/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<MedicalServiceType>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<MedicalServiceType>(`${environment.apiUrl}/api/medical-service-types/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: MedicalServiceType) {
        return this.http.post(`${environment.apiUrl}/api/medical-service-types`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: MedicalServiceType) {
        return this.http.put(`${environment.apiUrl}/api/medical-service-types/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/medical-service-types/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
