import { Component } from '@angular/core';
import {IdentityService} from "common";
import {Router} from "@angular/router";

@Component({
  selector: 'profile',
  templateUrl: './profile.component.html'
})
export class ProfileComponent {
  constructor(
    private authService: IdentityService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (!this.authService.isLogged)
      this.router.navigate(['/sign-in']);
  }
}
