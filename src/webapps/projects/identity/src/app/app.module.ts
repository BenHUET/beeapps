import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppComponent} from './app.component';
import {environment} from "../environments/environment";
import {NotFoundComponent, NavbarComponent, InputGroupComponent} from "common";
import {HttpClientModule} from "@angular/common/http";
import {CookieModule} from "ngx-cookie";
import {SignInComponent} from "../components/sign-in/sign-in.component";
import {ReactiveFormsModule} from "@angular/forms";
import {RouterModule, Routes} from "@angular/router";
import {ProfileComponent} from "../components/profile/profile.component";
import {NgbAlert} from "@ng-bootstrap/ng-bootstrap";
import {SignUpComponent} from "../components/sign-up/sign-up.component";
import {ValidateEmailComponent} from "../components/validate-email/validate-email.component";

const routes: Routes = [
  { path: 'sign-in', component: SignInComponent },
  { path: 'sign-up', component: SignUpComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'validate-email/:token', component: ValidateEmailComponent },
  { path: '',   redirectTo: '/sign-in', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    ProfileComponent,
    SignUpComponent,
    ValidateEmailComponent
  ],
  imports: [
    BrowserModule,
    NavbarComponent,
    HttpClientModule,
    CookieModule.withOptions(),
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
    InputGroupComponent,
    NgbAlert
  ],
  exports: [RouterModule],
  providers: [
    {provide: 'authEndpoint', useValue: environment.AUTH_ENDPOINT},
    {provide: 'cookieDomain', useValue: environment.COOKIE_DOMAIN}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
