import {DateTime} from "luxon";

export interface ContractorPage {
    title: string;
    bio: string;
};

export interface User {
    userName: string;
    name: string;
    contractorPage: ContractorPage | null;
};

export interface Category {
    id: number;
    name: string;
    description: string;
    allowedUsers: User[];
    everyoneAllowed: boolean;
    owner: User;
    maxAttendees: number;
    price: number;
};

export interface Appointment {
    id: number;
    startTime: DateTime;
    endTime: DateTime;
    category: Category;
    attendees: User[];
    maxAttendees: number;
};

export interface Report {
    owner: User;
    customer: User;
    entries: {
        category: Category;
        count: number;
    }[]; 
}

export interface UserSelfInfo {
    userName: string;
    name: string;
    email: string;
    contractorPage: ContractorPage | null;
};
