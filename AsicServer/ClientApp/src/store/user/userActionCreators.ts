import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import UserLogin from "../../models/UserLogin";
import { constants } from "../../constants/constant";
import { UserLoginResponse } from "../../models/UserLoginResponse";
import { login, createUsers } from "../../services/User";
import CreateUsers from "../../models/CreateUsers";

export const ACTIONS = {
    START_REQUEST_LOGIN:"START_REQUEST_LOGIN",
    STOP_REQUEST_LOGIN_WITH_ERRORS:"STOP_REQUEST_LOGIN_WITH_ERRORS",
    RECEIVE_SUCCESS_LOGIN:"RECEIVE_SUCCESS_LOGIN"

}

function startRequestLogin() {
    return {
        type: ACTIONS.START_REQUEST_LOGIN
    };
}

function stopRequestLoginWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS,
        errors
    };
}

function receiveSuccessLogin(userLoginResponse: UserLoginResponse) {
    return {
        type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
        user: userLoginResponse
    };
}

const requestLogin = (userLogin: UserLogin, redirect: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestLogin());

    const apiResponse: ApiResponse = await login(userLogin);
    if (apiResponse.success) {
        console.log(apiResponse.data);
        localStorage.setItem(constants.AUTH_IN_LOCAL_STORAGE, JSON.stringify(apiResponse.data.user));
        localStorage.setItem(constants.ACCESS_TOKEN, apiResponse.data.accessToken);
        dispatch(receiveSuccessLogin(apiResponse.data));
        redirect();
    } else {
        dispatch(stopRequestLoginWithError(apiResponse.errors));
    }
}

const requestCreateUsers = (newUsers: CreateUsers): AppThunkAction => async (dispatch, getState) => {
    const apiResponse: ApiResponse = await createUsers(newUsers);
    if(apiResponse.success) {

    } else {
        console.log(apiResponse.errors);
    }
}

export const userActionCreators = {
    requestLogin: requestLogin,
    requestCreateUsers: requestCreateUsers
};