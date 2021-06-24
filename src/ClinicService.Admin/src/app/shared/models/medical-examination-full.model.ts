import { MedicalExaminationDetail } from "./medical-examination-detail.model";
import { Prescription } from "./prescription.model";

export class MedicalExaminationFull {
    patientId: string;
    appointmentId: number | null;
    reappointmentId: number | null;
    statusCategoryId: string;
    detailRequestModels: string;
    prescriptionRequestModels: string;
    attachments: any[];
}
