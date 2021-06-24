import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User, Pagination, Role, UserPasswordRequest } from '../models';
import { UtilitiesService } from './utilities.service';

@Injectable({ providedIn: 'root' })
export class UsersService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient, private utilitiesService: UtilitiesService) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<User[]>(`${environment.apiUrl}/api/Users`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getAllByRoleId(roleId: string) {
        return this.http.get<User[]>(`${environment.apiUrl}/api/Users/get-by-role/${roleId}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getAllByRoleIdSearch(roleId: string, q: string, page: number, limit: number) {
        return this.http.get<Pagination<User>>(`${environment.apiUrl}/api/Users/get-by-role/${roleId}/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<User>) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<User>>(`${environment.apiUrl}/api/Users/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<User>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<User>(`${environment.apiUrl}/api/Users/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: User) {
        return this.http.post(`${environment.apiUrl}/api/Users`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: User) {
        return this.http.put(`${environment.apiUrl}/api/Users/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/Users/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getMenuByUserId(userId: string) {
        return this.http.get<Function[]>(`${environment.apiUrl}/api/users/${userId}/Menu`, { headers: this._sharedHeader })
            .pipe(map(res => {
                const functions = this.utilitiesService.UnflatteringForLeftMenu(res);
                return functions;
            }), catchError(this.handleError));
    }

    getRolesByUserId(userId: string) {
        return this.http.get<string[]>(`${environment.apiUrl}/api/users/${userId}/Roles`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    postRoleByUserId(userId: string, roleName: any) {
        return this.http.post(`${environment.apiUrl}/api/users/${userId}/Roles`,
            JSON.stringify(roleName), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteRoleByUserId(userId: string, roleName: any) {
        return this.http.delete(`${environment.apiUrl}/api/users/${userId}/Roles?roleName=${roleName}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    changePassword(id: string, entity: UserPasswordRequest) {
        return this.http.put(`${environment.apiUrl}/api/Users/${id}/change-password`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    resetPassword(id: string) {
        return this.http.put(`${environment.apiUrl}/api/Users/${id}/reset-password`, null, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
