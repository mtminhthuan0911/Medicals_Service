import { Injectable } from '@angular/core';
import { NotificationsService } from './notifications.service';

@Injectable({ providedIn: 'root' })
export class ErrorsService {
    constructor(
        private notificationsService: NotificationsService
    ) { }

    notifyErrors(errors: any): void {
        if (Array.isArray(errors)) {
            for (const err of errors) {
                this.notificationsService.notifyError(err);
            }
        } else {
            this.notificationsService.notifyError(errors);
        }
    }
}
