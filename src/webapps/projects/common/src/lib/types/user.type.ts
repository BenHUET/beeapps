import {Token} from "./token.type";

export class User {
  username?: string;
  email?: string;

  accessToken?: Token;
  refreshToken?: Token;

  constructor(username: string, email: string, accessToken: Token, refreshToken: Token) {
    this.username = username;
    this.email = email;
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
  }
}
