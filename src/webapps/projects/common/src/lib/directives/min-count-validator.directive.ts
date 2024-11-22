import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

function minCountValidator(pattern : RegExp, count: number, key: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (count === 0)
      return null;

    const groups = control.value.match(pattern);
    if (!groups || groups.length < count)
      return {
        [key]: {
          value: control.value,
          expected: count,
          found: groups ? groups.length : 0
        }
      };

    return null;
  };
}

export function minNumberValidator(count: number): ValidatorFn {
  return minCountValidator(/\d/g, count, "minNumber");
}

export function minSpecialCharsValidator(count: number): ValidatorFn {
  return minCountValidator(/[!"#$%&'()*+,-./:;<=>?@[\]^_`{|}~\\]/g, count, "minSpecialChar");
}

export function minLetterValidator(count: number): ValidatorFn {
  return minCountValidator(/[a-zA-Z]+/g, count, "minLetter");
}
