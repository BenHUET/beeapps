import {AbstractControl, FormGroup, ValidationErrors, ValidatorFn} from '@angular/forms';

export function identicalValidator(form: FormGroup, controlName: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const control2 = form.get(controlName);

    if (!control || !control2 || control.value === control2.value)
      return null;

    return {identical: {value: control.value}};
  };
}
