import { Component, OnInit } from '@angular/core';
import { noTransition } from '../router.animations';
import { NgxSpinnerService } from 'ngx-spinner';
import { AuthService } from '../shared/services';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    animations: [noTransition()]
})
export class LoginComponent implements OnInit {
    constructor(
        private authService: AuthService,
        private spinner: NgxSpinnerService
    ) { }

    ngOnInit() { }

    onLoggedin() {
        this.spinner.show();
        this.authService.login();
    }
}
