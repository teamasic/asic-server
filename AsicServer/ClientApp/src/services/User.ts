import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import UserLogin from "../models/UserLogin";
import { routes } from "../constants/routes";
import CreateUsers from "../models/CreateUsers";

const baseRoute  = routes.BASE_ROUTE + "user";
const apify = (path: string) => `${baseRoute}/${path}`;

export const login = async (userLogin: UserLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login'), userLogin);
    return await response.data;
};

export const createUsers = async (zipFile: File, csvFile: File): Promise<ApiResponse> => {
    var formData = new FormData();
    formData.append("zipFile", zipFile);
    formData.append("users", csvFile);
    const apiResponse = await axios.post(baseRoute, formData);
    return await apiResponse.data;
}



