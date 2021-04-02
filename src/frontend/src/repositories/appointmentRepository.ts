import { AppointmentDTO } from "../logic/dtos";
import { Appointment } from "../logic/entities";
import { ResultPromise, Unit } from "../utilities/result";
import { ICategoryRepository } from "./categoryRepository";
import { IUserRepository } from "./userRepository";
import { parseResponseAs, safeApiFetchAs, safeApiFetchAsUnit, safeApiFetchWithBodyAs, safeApiFetchWithBodyAsUnit } from "./utilities";
import {Config} from "../config";

export interface IAppointmentRepository {
    getById(id: number): ResultPromise<Appointment,Error>;
    getContractorsAppointments(contractorUserName: string): ResultPromise<Appointment[],Error>;
    getBooked(): ResultPromise<Appointment[],Error>;
    book(id: number): ResultPromise<Unit,Error>;
    unBook(id: number): ResultPromise<Unit,Error>;
    create(dto: AppointmentDTO): ResultPromise<Appointment,Error>;
    delete(id: number): ResultPromise<Unit,Error>;
};

export class AppointmentRepository implements IAppointmentRepository {
    constructor(
        private readonly config: Config,
        private readonly userRepo: IUserRepository, 
        private readonly categoryRepo: ICategoryRepository
    ) {}

    private dtoToEntity = (dto: AppointmentDTO): ResultPromise<Appointment,Error> =>
        ResultPromise.ok<AppointmentDTO,Error>(dto)
            .andThen(appointmentDto => {
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

    private dtosToEntities = (dtos: AppointmentDTO[]): ResultPromise<Appointment[],Error> =>
        ResultPromise.all(dtos.map(this.dtoToEntity));
        

    getById = (id: number): ResultPromise<Appointment,Error> =>
        safeApiFetchAs<AppointmentDTO>(`${this.config.apiUrl}/Appointment/${id}`, 'GET')
            .andThen(this.dtoToEntity);

    getContractorsAppointments = (contractorUserName: string): ResultPromise<Appointment[],Error> =>
        safeApiFetchAs<AppointmentDTO[]>(`${this.config.apiUrl}/Appointment/Contractor/${contractorUserName}`, 'GET')
            .andThen(this.dtosToEntities);

    getBooked = (): ResultPromise<Appointment[],Error> =>
        safeApiFetchAs<AppointmentDTO[]>(`${this.config.apiUrl}/Appointment/Booked`, 'GET')
            .andThen(this.dtosToEntities);

    book = (id: number): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/Appointment/${id}/Book`, 'POST');

    unBook = (id: number): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/Appointment/${id}/UnBook`, 'POST');

    create = (dto: AppointmentDTO): ResultPromise<Appointment,Error> =>
        safeApiFetchWithBodyAs<AppointmentDTO>(`${this.config.apiUrl}/Appointment`, 'POST', dto)
            .andThen(this.dtoToEntity);

    delete = (id: number): ResultPromise<Unit,Error> =>
        safeApiFetchAsUnit(`${this.config.apiUrl}/Appointment/${id}`, 'DELETE');
};
