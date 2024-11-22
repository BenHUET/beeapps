import {Component, Input} from '@angular/core';
import {FormControl, ReactiveFormsModule} from "@angular/forms";
import {NgClass, NgFor, NgIf} from "@angular/common";

@Component({
  selector: 'input-group',
  templateUrl: './input-group.component.html',
  imports: [ReactiveFormsModule, NgIf, NgClass, NgFor],
  standalone: true
})
export class InputGroupComponent {
  @Input('control') control!: FormControl;
  @Input('icon') icon: string = '?';
  @Input('type') type!: string;
  @Input('placeholder') placeholder: string = '';
  @Input('cssClasses') cssClasses: string = '';
}
