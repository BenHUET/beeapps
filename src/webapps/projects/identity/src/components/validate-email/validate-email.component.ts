import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {IdentityService, User} from "common";

@Component({
  selector: 'validate-email',
  templateUrl: './validate-email.component.html'
})
export class ValidateEmailComponent {
  isWaiting = true;
  error? : string;

  constructor(
    private authService: IdentityService,
    private route:ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.authService.validateEmail(this.route.snapshot.paramMap.get('token')!)
      .subscribe({
        next: (data: User) => {
          this.isWaiting = false;
        },
        error: (err: Error) => {
          this.error = err.message;
          this.isWaiting = false;
        }
      });
  }
}
