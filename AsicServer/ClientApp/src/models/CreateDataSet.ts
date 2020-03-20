import RollNumber from "./RollNumber";

export default interface DataSet {
    id: number;
    name: string;
    rollNumbers: RollNumber[];
}