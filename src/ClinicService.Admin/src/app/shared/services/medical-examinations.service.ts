import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { Pagination, ClinicBranch, Appointment, MedicalExamination, MedicalExaminationFull, MedicalExaminationDetail, Prescription } from '../models';
import { environment } from '@environments/environment';
import { catchError, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class MedicalExaminationsService extends BaseService {
    private _sharedHeader = new HttpHeaders();

    constructor(private http: HttpClient) {
        super();
        this._sharedHeader = this._sharedHeader.set('Content-Type', 'application/json');
    }

    getAll() {
        return this.http.get<MedicalExamination[]>(`${environment.apiUrl}/api/medical-examinations`, { headers: this._sharedHeader })
            .pipe(map((res: MedicalExamination[]) => {
                return res;
            }), catchError(this.handleError));
    }

    getSearch(q: string, page: number, limit: number) {
        return this.http.get<Pagination<MedicalExamination>>(`${environment.apiUrl}/api/medical-examinations/search?q=${q}&page=${page}&limit=${limit}`,
            { headers: this._sharedHeader })
            .pipe(map((res: Pagination<MedicalExamination>) => {
                return res;
            }), catchError(this.handleError));
    }

    getDetail(id: number) {
        return this.http.get<MedicalExamination>(`${environment.apiUrl}/api/medical-examinations/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getDetailFromAppointmentId(appointmentId: number) {
        return this.http.get<MedicalExamination>(`${environment.apiUrl}/api/medical-examinations/from-appointments/${appointmentId}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    getDetailFromReappointmentId(reappointmentId: number) {
        return this.http.get<MedicalExamination>(`${environment.apiUrl}/api/medical-examinations/from-reappointments/${reappointmentId}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    add(entity: MedicalExamination) {
        return this.http.post(`${environment.apiUrl}/api/medical-examinations`, JSON.stringify(entity), { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    addFull(entity: FormData) {
        return this.http.post(`${environment.apiUrl}/api/medical-examinations/full`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    update(id: number, entity: FormData) {
        return this.http.put(`${environment.apiUrl}/api/medical-examinations/${id}`, entity, { reportProgress: true, observe: 'events' })
            .pipe(catchError(this.handleError));
    }

    updateStatus(id: number, statusCategoryId: string) {
        return this.http.put(`${environment.apiUrl}/api/medical-examinations/${id}/change-status/${statusCategoryId}`, null,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    delete(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/medical-examinations/${id}`, { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteAttachment(medicalExaminationId: number, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/attachments/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    // via Details
    getDetails(medicalExaminationId: number) {
        return this.http.get<MedicalExaminationDetail[]>(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/details`,
            { headers: this._sharedHeader })
            .pipe(map((res: MedicalExaminationDetail[]) => {
                return res;
            }), catchError(this.handleError));
    }

    addDetail(medicalExaminationId: number, entity: MedicalExaminationDetail) {
        return this.http.post(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/details`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    updateDetail(medicalExaminationId: number, id: number, entity: MedicalExaminationDetail) {
        return this.http.put(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/details/${id}`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deleteDetail(medicalExaminationId: number, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/details/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    // via Prescriptions
    getPrescriptions(medicalExaminationId: number) {
        return this.http.get<Prescription[]>(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/prescriptions`,
            { headers: this._sharedHeader })
            .pipe(map((res: Prescription[]) => {
                return res;
            }), catchError(this.handleError));
    }

    addPrescription(medicalExaminationId: number, entity: Prescription) {
        return this.http.post(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/prescriptions`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    updatePrescription(medicalExaminationId: number, id: number, entity: Prescription) {
        return this.http.put(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/prescriptions/${id}`, JSON.stringify(entity),
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    deletePrescription(medicalExaminationId: number, id: number) {
        return this.http.delete(`${environment.apiUrl}/api/medical-examinations/${medicalExaminationId}/prescriptions/${id}`,
            { headers: this._sharedHeader })
            .pipe(catchError(this.handleError));
    }

    // via Users
    getAllByPatientId(patientId: string) {
        return this.http.get<MedicalExamination[]>(`${environment.apiUrl}/api/medical-examinations/patients/${patientId}`,
            { headers: this._sharedHeader })
            .pipe(map((res: MedicalExamination[]) => {
                return res;
            }), catchError(this.handleError));
    }
}
