import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService, UsersService, ErrorsService } from '@app/shared/services';
import { Function } from '@app/shared/models';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  isActive: boolean;
  showMenu: string;
  isToggle: boolean[] = []
  pushRightClass: string;

  public functions: Function[];

  constructor(
    private translate: TranslateService,
    public router: Router,
    private authService: AuthService,
    private usersService: UsersService,
    private errorsService: ErrorsService
  ) {
    this.router.events.subscribe((val) => {
      if (val instanceof NavigationEnd && window.innerWidth <= 992 && this.isToggled()) {
        this.toggleSidebar();
      }
    });
    this.loadMenu();
  }

  ngOnInit() {
    this.isActive = false;
    this.showMenu = '';
    this.pushRightClass = 'push-right';
  }

  loadMenu() {
    const profile = this.authService.profile;
    this.usersService.getMenuByUserId(profile.sub).subscribe((res: Function[]) => {
      this.functions = res;
    },
      errors => {
        this.errorsService.notifyErrors(errors);
      });
  }

  eventCalled() {
    this.isActive = !this.isActive;
  }

  addExpandClass(element: any) {
    if (element === this.showMenu) {
      this.showMenu = '0';
    } else {
      this.showMenu = element;
      for (let i = 0; i < this.isToggle.length; i++)
        this.isToggle[i] = false;
    }
  }

  isToggled(): boolean {
    const dom: Element = document.querySelector('body');
    return dom.classList.contains(this.pushRightClass);
  }

  toggleSidebar() {
    const dom: any = document.querySelector('body');
    dom.classList.toggle(this.pushRightClass);
  }

  rltAndLtr() {
    const dom: any = document.querySelector('body');
    dom.classList.toggle('rtl');
  }

  changeLang(language: string) {
    this.translate.use(language);
  }

  async onLoggedout() {
    await this.authService.signout();
  }
}
