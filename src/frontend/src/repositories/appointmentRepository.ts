import { AppointmentDTO } from "../logic/dtos";
import { Appointment } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { ICategoryRepository } from "./categoryRepository";
import { IUserRepository } from "./userRepository";
import { safeApiFetch, safeApiFetchWithBody } from "./utilities";

export interface IAppointmentRepository {
    getById(id: number): ResultPromise<Appointment,Error>;
    book(id: number): ResultPromise<void,Error>;
    unBook(id: number): ResultPromise<void,Error>;
};

export class AppointmentRepository implements IAppointmentRepository {
    userRepo: IUserRepository;
    categoryRepo: ICategoryRepository;

    constructor(userRepository: IUserRepository, categgoryRepository: ICategoryRepository) {
        this.userRepo = userRepository;
        this.categoryRepo = categgoryRepository;
    }

    getById = (id: number): ResultPromise<Appointment,Error> =>
        safeApiFetch(`https://localhost:44347/Appointment/${id}`, 'GET')
            .andThenAsync(async response => {
                const appointmentDto = await response.json() as AppointmentDTO;
                const categoryResult = this.categoryRepo.getById(appointmentDto.categoryId);
                const attendeeResults = ResultPromise.all(
                    appointmentDto.attendeeUserNames.map(userName => this.userRepo.getByUserName(userName))
                );
                
                return categoryResult
                    .andThen(category => 
                        attendeeResults.map(attendees => (
                            {
                            id: appointmentDto.id,
                            maxAttendees: appointmentDto.id,
                            startTime: new Date(appointmentDto.startTime),
                            endTime: new Date(appointmentDto.endTime),
                            category,
                            attendees,
                        } as Appointment))
                    );
            });

    book = (id: number): ResultPromise<void,Error> =>
        safeApiFetchWithBody(`https://localhost:44347/Appointment/${id}/Book`, 'POST')
            .mapAsync(async _response => {});

    unBook = (id: number): ResultPromise<void,Error> =>
        safeApiFetchWithBody(`https://localhost:44347/Appointment/${id}/UnBook`, 'POST')
            .mapAsync(async _response => {});
};
