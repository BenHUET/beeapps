import {Component, Input} from '@angular/core';
import {NgbNav} from "@ng-bootstrap/ng-bootstrap";
import {RouterModule} from "@angular/router";

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html',
  imports: [
    NgbNav,
    RouterModule
  ],
  standalone: true
})
export class NavbarComponent {
  @Input('service') serviceName: string = '';
  @Input('masterUrl') masterUrl: string = '';
}
