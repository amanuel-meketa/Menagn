export interface BackendUser {
  sub: string;
  email?: string;
  preferred_username?: string;
  name?: string;
  given_name?: string;
  family_name?: string;
  email_verified?: boolean;
}

export interface AuthMeResponse {
  success: boolean;
  user: {
    access_token: string;
    user: BackendUser;
    id_token: any;
  };
}

export interface GetCurrentUser {
  userId: string;
  email?: string;
  username?: string;
  fullName?: string;
  firstName?: string;
  lastName?: string;
}
