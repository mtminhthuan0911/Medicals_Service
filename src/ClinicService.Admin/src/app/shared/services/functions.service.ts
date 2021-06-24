import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Function, Pagination, CommandInFunction } from '../models';

@Injectable({ providedIn: 'root' })
export class FunctionsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<Function[]>(`${environment.apiUrl}/api/Functions`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<Function>(`${environment.apiUrl}/api/Functions/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: Function) {
        return this.http.post(`${environment.apiUrl}/api/Functions`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: Function) {
        return this.http.put(`${environment.apiUrl}/api/Functions/${id}`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/Functions/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getCommandsByFunctionId(functionId: string) {
        return this.http.get(`${environment.apiUrl}/api/Functions/${functionId}/Commands`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    postCommandByFunctionId(functionId: string, entity: CommandInFunction) {
        return this.http.post(`${environment.apiUrl}/api/Functions/${functionId}/Commands`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteCommandByFunctionId(functionId: string, commandId: string) {
        return this.http.delete(`${environment.apiUrl}/api/Functions/${functionId}/Commands/${commandId}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
