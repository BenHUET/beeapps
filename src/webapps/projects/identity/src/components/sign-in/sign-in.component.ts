import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ErrorWithStatusCode, IdentityService, User} from "common";
import {Router} from "@angular/router";

@Component({
  selector: 'sign-in',
  templateUrl: './sign-in.component.html'
})
export class SignInComponent {
  signinForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  });
  isWaiting = false;
  mailSent = false;
  error? : ErrorWithStatusCode;

  constructor(
    private authService: IdentityService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (this.authService.isLogged)
      this.router.navigate(['/profile']);
  }

  onSubmit() : void {
    this.isWaiting = true;
    this.mailSent = false;

    this.authService.authenticate(this.signinForm.controls.email.value!, this.signinForm.controls.password.value!)
      .subscribe({
        next: (_: User) => {
          this.router.navigate(['/profile']);
        },
        error: (err: ErrorWithStatusCode) => {
          this.error = err;
          this.isWaiting = false;
        }
      });
  }

  resendEmail() : void {
    this.isWaiting = true;

    this.authService.resendEmail(this.signinForm.controls.email.value!, this.signinForm.controls.password.value!)
      .subscribe({
        next: () => {
          this.mailSent = true;
          this.isWaiting = false;
          this.error = undefined;
        },
        error: (err: Error) => {
          this.error = { message: err.message, status: 0 };
          this.isWaiting = false;
        }
      });
  }
}
