import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/shared/services';

@Component({
    selector: 'app-access-denied',
    templateUrl: './access-denied.component.html',
    styleUrls: ['./access-denied.component.scss']
})
export class AccessDeniedComponent implements OnInit {
    constructor(
        private authService: AuthService
    ) { }

    ngOnInit() { }

    async onLoggedOut() {
        await this.authService.signout();
    }
}
