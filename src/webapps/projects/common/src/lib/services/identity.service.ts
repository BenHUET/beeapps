import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpParams} from '@angular/common/http';
import {catchError, map, Observable, throwError} from "rxjs";
import {ErrorWithStatusCode, User, UserAuthResponseDTO} from "common";
import {CookieService} from "ngx-cookie";

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  private cookieUserKey : string = 'user';

  public user?: User;
  get isLogged(): boolean { return this.user != undefined; }

  constructor(
    private httpClient: HttpClient,
    private cookieService: CookieService,
    @Inject('authEndpoint') private authEndpoint: string,
    @Inject('cookieDomain') private cookieDomain: string
  ) {
    this.user = this.cookieService.getObject(this.cookieUserKey);
  }

  authenticate(email: string, password: string): Observable<User> {
    return this.httpClient
      .post<UserAuthResponseDTO>(this.authEndpoint + "/users/authenticate", {email, password})
      .pipe(
        map(dto => {
          this.user = new User(dto.username, dto.email, dto.accessToken, dto.refreshToken);
          this.cookieService.putObject(this.cookieUserKey, this.user, {domain: this.cookieDomain});
          return this.user;
        }),
        catchError((err: HttpErrorResponse): Observable<never> => {
          let msg = 'something went wrong with the server';

          if (err.status === 401)
            msg = 'invalid credentials';
          else if (err.status === 403)
            msg = 'email not confirmed, please check your mailbox';
          else if (err.status === 0)
            msg = 'something went wrong with your request';

          return throwError(() => {
            const error : ErrorWithStatusCode = {message:msg, status: err.status};
            return error;
          });
        })
      );
  }

  create(username: string, email: string, password: string): Observable<User> {
    return this.httpClient
      .post(this.authEndpoint + "/users", {username, email, password})
      .pipe(
        catchError((err : HttpErrorResponse): Observable<never> => {
          let msg = 'something went wrong with the server';

          if (err.status === 400) {
            msg = 'something went wrong with your request';

            if (err.error.errors.Username)
              msg = 'username already taken';
          }

          return throwError(() => new Error(msg));
        })
      );
  }

  validateEmail(token: string): Observable<User> {
    let queryParams = new HttpParams();
    queryParams = queryParams.append("Token", token);

    return this.httpClient
      .get(this.authEndpoint + "/users/validate-email", {params: queryParams})
      .pipe(
        catchError((err : HttpErrorResponse): Observable<never> => {
          let msg = 'something went wrong with the server';

          if (err.status === 400) {
            msg = 'invalid link';
          }

          return throwError(() => new Error(msg));
        })
      );
  }

  resendEmail(email: string, password: string) {
    return this.httpClient
      .post(this.authEndpoint + "/users/resend-email", {email, password})
      .pipe(
        catchError((err: HttpErrorResponse): Observable<never> => {
          let msg = 'unable to send a new link';

          if (err.status === 500)
            msg = 'something went wrong with the server';

          return throwError(() => new Error(msg));
        })
      );
  }
}
