import { MedicalExaminationDetail } from "./medical-examination-detail.model";
import { Prescription } from "./prescription.model";

export class MedicalExamination {
    id: number;
    createdDate: string;
    modifiedDate: string;
    patientId: string;
    patientFullName: string;
    statusCategoryId: string;
    statusCategoryName: string;
    details: MedicalExaminationDetail[];
    prescriptions: Prescription[];
    attachments: any[];
    appointmentId: number;
    reappointmentId: number;
}
