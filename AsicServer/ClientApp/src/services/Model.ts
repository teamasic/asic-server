import ApiResponse from "../models/ApiResponse";
import axios from "axios";
import { routes } from "../constants/routes";

const baseRoute  = routes.BASE_ROUTE + "model";
const apify = (path: string) => `${baseRoute}/${path}`;

export const train = async (): Promise<ApiResponse> => {
    const response = await axios.post(baseRoute);
    return await response.data;
};

export const addEmbeddings = async (codes: string[]): Promise<ApiResponse> => {
    const response = await axios.put(baseRoute, {
        codes
    });
    return await response.data;
};

export const removeEmbeddings = async (codes: string[]): Promise<ApiResponse> => {
    const response = await axios.post(apify('remove'), {
        codes
    });
    return await response.data;
};

