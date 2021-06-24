import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ClinicBranchesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<ClinicBranch[]>(`${environment.apiUrl}/api/clinic-branches`, { headers: this._sharedHeader })
            .pipe(map((res: ClinicBranch[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<ClinicBranch>>(`${environment.apiUrl}/api/clinic-branches/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<ClinicBranch>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: number) {
        return this.http.get<ClinicBranch>(`${environment.apiUrl}/api/clinic-branches/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: ClinicBranch) {
        return this.http.post(`${environment.apiUrl}/api/clinic-branches`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: number, entity: ClinicBranch) {
        return this.http.put(`${environment.apiUrl}/api/clinic-branches/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/clinic-branches/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
