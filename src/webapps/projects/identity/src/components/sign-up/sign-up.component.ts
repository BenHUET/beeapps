import { Component } from '@angular/core';
import {AbstractControl, FormControl, FormGroup, ValidationErrors, Validators} from "@angular/forms";
import {
  alphanumericValidator,
  IdentityService,
  minLetterValidator,
  minNumberValidator,
  minSpecialCharsValidator, User
} from "common";
import {Router} from "@angular/router";
import {identicalValidator} from "../../../../common/src/lib/directives/identical-validator.directive";

@Component({
  selector: 'sign-up',
  templateUrl: './sign-up.component.html'
})
export class SignUpComponent {
  signupForm = new FormGroup({
    username: new FormControl('', [
      Validators.required,
      Validators.minLength(4),
      Validators.maxLength(16),
      alphanumericValidator()
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    emailConfirmation: new FormControl('', [
      Validators.required,
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(64),
      minNumberValidator(1),
      minSpecialCharsValidator(1),
      minLetterValidator(1),
      (control: AbstractControl): ValidationErrors | null => {
        const valid = /^[A-Za-z\d !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~\\]+$/.test(control.value);
        return valid ? null : {passwordInvalidCharacters: {value: control.value}};
      }
    ]),
    passwordConfirmation: new FormControl('', [
      Validators.required
    ]),
  });

  isWaiting = false;
  error? : string;

  constructor(
    private authService: IdentityService,
    private router: Router
  ) {
    this.signupForm.controls.emailConfirmation.addValidators(identicalValidator(this.signupForm, 'email'));
    this.signupForm.controls.passwordConfirmation.addValidators(identicalValidator(this.signupForm, 'password'));
  }

  ngOnInit(): void {
    if (this.authService.isLogged)
      this.router.navigate(['/profile']);
  }

  onSubmit() : void {
    this.isWaiting = true;

    this.authService.create(this.signupForm.controls.username.value!, this.signupForm.controls.email.value!, this.signupForm.controls.password.value!)
      .subscribe({
        next: (data: User) => {
          console.log(data);
        },
        error: (err: Error) => {
          this.error = err.message;
          this.isWaiting = false;
        }
    });
  }
}
