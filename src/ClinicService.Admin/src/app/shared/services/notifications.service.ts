import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable()
export class NotificationsService {
    constructor(private messageService: MessageService) { }

    notifySuccess(detail: string): void {
        this.messageService.add({ severity: 'success', summary: 'Thành công!', detail: detail });
    }

    notifyError(detail: string): void {
        this.messageService.add({ severity: 'error', summary: 'Lỗi!', detail: detail });
    }
}
