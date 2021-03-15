import { AppointmentDTO } from "../logic/dtos";
import { Appointment } from "../logic/entities";
import { ResultPromise, Unit } from "../utilities/result";
import { ICategoryRepository } from "./categoryRepository";
import { IUserRepository } from "./userRepository";
import { parseResponseAs, safeApiFetchAs, safeApiFetchWithBodyAs, safeApiFetchWithBodyAsUnit } from "./utilities";

export interface IAppointmentRepository {
    getById(id: number): ResultPromise<Appointment,Error>;
    getContractorsAppointments(contractorUserName: string): ResultPromise<Appointment[],Error>;
    book(id: number): ResultPromise<Unit,Error>;
    unBook(id: number): ResultPromise<Unit,Error>;
};

export class AppointmentRepository implements IAppointmentRepository {
    constructor(
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

    getById = (id: number): ResultPromise<Appointment,Error> =>
        safeApiFetchAs<AppointmentDTO>(`https://localhost:44347/Appointment/${id}`, 'GET')
            .andThen(this.dtoToEntity);

    getContractorsAppointments = (contractorUserName: string): ResultPromise<Appointment[],Error> =>
        safeApiFetchAs<AppointmentDTO[]>(`https://localhost:44347/Appointment/Contractor/${contractorUserName}`, 'GET')
            .andThen(appointmentsDtos => ResultPromise.all(appointmentsDtos.map(this.dtoToEntity)));

    book = (id: number): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`https://localhost:44347/Appointment/${id}/Book`, 'POST');

    unBook = (id: number): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`https://localhost:44347/Appointment/${id}/UnBook`, 'POST');
};
