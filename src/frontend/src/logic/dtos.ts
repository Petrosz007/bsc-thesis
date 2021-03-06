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
    startTime: Date;
    endTime: Date;
    categoryId: number;
    attendeeUserNames: string[];
    maxAttendees: number;
};