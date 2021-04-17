import { ContractorPage } from "./entities";

export interface CategoryDTO {
    id: number;
    name: string;
    description: string;
    allowedUserNames: string[];
    everyoneAllowed: boolean;
    ownerUserName: string;
    maxAttendees: number;
    price: number;
};

export interface AppointmentDTO {
    id: number;
    startTime: string;
    endTime: string;
    categoryId: number;
    attendeeUserNames: string[];
    maxAttendees: number;
};

export interface LoginDTO {
    userName: string;
    password: string;
}

export interface IsLoggedInDTO {
    isLoggedIn: boolean;
    userName: string|null;
}

export interface RegisterDTO {
    userName: string,
    email: string,
    name: string,
    password: string,
    passwordConfirmation: string,
    contractorPage: ContractorPage | null,
}

export interface UserEditDTO {
    email: string;
    name: string;
    contractorPage: ContractorPage | null;
}
