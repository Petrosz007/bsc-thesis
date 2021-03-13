import { AppointmentDTO } from "src/logic/dtos";
import { Appointment } from "src/logic/entities";
import { ICategoryRepository } from "./categoryRepository";
import { IUserRepository } from "./userRepository";
import { apiFetch, apiFetchWithBody } from "./utilities";

export interface IAppointmentRepository {
    getById(id: number): Promise<Appointment>;
    book(id: number): Promise<void>;
    unBook(id: number): Promise<void>;
};

export class AppointmentRepository implements IAppointmentRepository {
    userRepo: IUserRepository;
    categoryRepo: ICategoryRepository;

    constructor(userRepository: IUserRepository, categgoryRepository: ICategoryRepository) {
        this.userRepo = userRepository;
        this.categoryRepo = categgoryRepository;
    }

    async getById(id: number): Promise<Appointment> {
        const response = await apiFetch(`https://localhost:44347/Appointment/${id}`, 'GET');
        if(!response.ok) {
            throw new Error(`${response.status}: ${await response.text()}`)
        }
        
        const appointmentDto = await response.json() as AppointmentDTO;
        const category = this.categoryRepo.getById(appointmentDto.categoryId);
        const attendees = Promise.all(appointmentDto.attendeeUserNames.map(userName => this.userRepo.getByUserName(userName)));

        const appointment: Appointment = {
            id: appointmentDto.id,
            maxAttendees: appointmentDto.id,
            startTime: new Date(appointmentDto.startTime),
            endTime: new Date(appointmentDto.endTime),
            category: await category,
            attendees: await attendees,
        };

        return appointment;
    }

    async book(id: number): Promise<void> {
        const response = await apiFetchWithBody(`https://localhost:44347/Appointment/${id}/Book`, 'POST');
        if(!response.ok) {
            throw new Error(`${response.status}: ${await response.text()}`)
        }

        return;
    }

    async unBook(id: number): Promise<void> {
        const response = await apiFetchWithBody(`https://localhost:44347/Appointment/${id}/UnBook`, 'POST');
        if(!response.ok) {
            throw new Error(`${response.status}: ${await response.text()}`)
        }

        return;
    }
};
