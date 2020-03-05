import User from "./User";

export interface UserLoginResponse{
    user: User,
    roles: string[],
    accessToken: string
}