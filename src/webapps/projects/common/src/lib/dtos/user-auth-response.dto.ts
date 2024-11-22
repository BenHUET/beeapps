import {Token} from "../types/token.type";

export interface UserAuthResponseDTO {
  username: string;
  email: string;
  accessToken: Token;
  refreshToken: Token;
}
