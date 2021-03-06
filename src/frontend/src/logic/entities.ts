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
    startTime: Date;
    endTime: Date;
    category: Category;
    attendees: User[];
    maxAttendees: number;
};