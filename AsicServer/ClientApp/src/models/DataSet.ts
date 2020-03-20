import User from "./User";

export default interface DataSet {
    id: number;
    name: string;
    users: User[]
}