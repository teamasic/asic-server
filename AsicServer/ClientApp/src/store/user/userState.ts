import User from "../../models/User";

export interface UserState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    currentUser: User;
    roles: string[],
    accessToken: string,
    isLogin: boolean;
    errors: any[]
}