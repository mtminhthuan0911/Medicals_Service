import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Command } from '../models';

@Injectable({ providedIn: 'root' })
export class CommandsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<Command[]>(`${environment.apiUrl}/api/Commands`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
