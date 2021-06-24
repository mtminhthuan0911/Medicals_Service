import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Role, Pagination, PermissionUpdateRequest } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class RolesService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<Role[]>(`${environment.apiUrl}/api/Roles`, { headers: this._sharedHeader })
            .pipe(map((res: Role[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<Role>>(`${environment.apiUrl}/api/Roles/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<Role>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<Role>(`${environment.apiUrl}/api/Roles/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: Role) {
        return this.http.post(`${environment.apiUrl}/api/Roles`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: Role) {
        return this.http.put(`${environment.apiUrl}/api/Roles/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/Roles/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getPermissionByRoleId(roleId: string) {
        return this.http.get(`${environment.apiUrl}/api/Roles/${roleId}/Permissions`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    putPermissionByRoleId(roleId: string, entity: PermissionUpdateRequest) {
        return this.http.put(`${environment.apiUrl}/api/Roles/${roleId}/Permissions`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
