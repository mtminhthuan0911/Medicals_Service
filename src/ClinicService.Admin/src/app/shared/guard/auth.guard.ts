import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services';
import { CommandsConstant } from '../constants';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private authService: AuthService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAuthenticated()) {
      const functionCode = route.data['functionCode'] as string;
      const permission = JSON.parse(this.authService.profile.permission);
      if (permission && permission.filter(x => x === functionCode + '_' + CommandsConstant.READ_ACTION).length > 0) {
        return true;
      } else {
        this.router.navigate(['/access-denied'], {
          queryParams: { redirect: state.url }
        });
        return false;
      }
    }
    this.router.navigate(['/login'], { queryParams: { redirect: state.url }, replaceUrl: true });
    return false;
  }

}