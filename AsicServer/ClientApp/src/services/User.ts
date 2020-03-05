import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import UserLogin from "../models/UserLogin";
import { routes } from "../constants/routes";

const baseRoute  = routes.BASE_ROUTE + "user";
const apify = (path: string) => `${baseRoute}/${path}`;

export const login = async (userLogin: UserLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login'), userLogin);
    return await response.data;
};



