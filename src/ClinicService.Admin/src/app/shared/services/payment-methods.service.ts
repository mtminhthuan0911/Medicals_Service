import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch, PaymentMethod } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PaymentMethodsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<PaymentMethod[]>(`${environment.apiUrl}/api/payment-methods`, { headers: this._sharedHeader })
            .pipe(map((res: PaymentMethod[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<PaymentMethod>>(`${environment.apiUrl}/api/payment-methods/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<PaymentMethod>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: string) {
        return this.http.get<PaymentMethod>(`${environment.apiUrl}/api/payment-methods/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: FormData) {
        return this.http.post(`${environment.apiUrl}/api/payment-methods`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    update(id: string, entity: FormData) {
        return this.http.put(`${environment.apiUrl}/api/payment-methods/${id}`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/api/payment-methods/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteAttachment(paymentMethodId: string, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/payment-methods/${paymentMethodId}/attachments/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }
}
