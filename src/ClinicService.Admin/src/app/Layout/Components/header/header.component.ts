import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '@app/shared/services';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ChangePasswordFormComponent } from '@app/layout/systems/users/components/change-password-form/change-password-form.component';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
    private subscription = new Subscription();

    public pushRightClass: string;
    private ref: DynamicDialogRef;

    public waitedLendingBaseQuantityComing: number;
    public hasNotifications = false;

    userName: string;
    isAuthenticated: boolean;

    constructor(
        public router: Router,
        private authService: AuthService,
        private dialogService: DialogService,
    ) {
        this.subscription = this.authService.authNavStatus$.subscribe(status => this.isAuthenticated = status);
        this.userName = this.authService.name;
    }

    ngOnInit() {
        this.pushRightClass = 'push-right';
        // this.subscription.add(this.lendingBasesService.quantityComingLateNotification()
        //     .subscribe((res: LendingBaseNotificationQuantityComingLate) => {
        //         this.lendingBaseQuantityComingLate = new LendingBaseNotificationQuantityComingLate();
        //         this.lendingBaseQuantityComingLate.expirationDateComing = res.expirationDateComing;
        //         this.lendingBaseQuantityComingLate.expirationDateLate = res.expirationDateLate;
        //     }));
        // this.subscription.add(this.waitedLendingBasesService.quantityComingNotification()
        //     .subscribe((res: WaitedLendingBaseNotificationQuantityComing) => {
        //         this.waitedLendingBaseQuantityComing = res.expectationDateComing;
        //     }));
    }

    toggleSidebar() {
        const dom: any = document.querySelector('body');
        dom.classList.toggle(this.pushRightClass);
    }

    async onLoggedOut() {
        await this.authService.signout();
    }

    onShowChangePasswordModal() {
        const data = {
            entityId: this.authService.profile.sub
        };

        this.ref = this.dialogService.open(ChangePasswordFormComponent, {
            data: data,
            header: 'Thay đổi mật khẩu',
            width: '64%'
        });

        this.ref.onClose.subscribe((res) => {
            if (res) {
                this.authService.signout();
            }
        });
    }
}
