import React, { useContext, useEffect } from 'react';
import { Appointment } from '../logic/entities';
import {LoggedIn, LoggedOut, LoginContext} from './contexts/LoginProvider';
import { Failed, Loading, useApiCall } from '../hooks/apiCallHooks';
import { DIContext } from './contexts/DIContext';
import { DataContext } from './contexts/DataProvider';

import './AppointmentCard.scss';
import { NotificationContext } from './contexts/NotificationProvider';

const HourDuration = ({ startTime, endTime }: { startTime: Date, endTime: Date }) => {
    const minutes = Math.floor((endTime.getTime() - startTime.getTime()) / (1000 * 60));
    if(minutes < 60) {
        return <>{minutes} perc</>;
    }

    const hours = (minutes / 60).toFixed(1).replace('.0','');
    return <>{hours} óra</>;
}

const FormattedDate = ({ date }: { date: Date }) => {
    const padNum = (x: number) => x.toFixed(0).padStart(2, '0');
    const year = date.getFullYear();
    const month = padNum(date.getMonth() + 1);
    const day = padNum(date.getDate());
    const hour = padNum(date.getHours());
    const minute = padNum(date.getMinutes());
    return <>{year}.{month}.{day} {hour}:{minute}</>;
}

const BookButton = ({ appointment }: { appointment: Appointment }) => {
    const { loginState } = useContext(LoginContext);
    const { dataDispatch } = useContext(DataContext);
    const { notificationDispatch } = useContext(NotificationContext);
    
    const { appointmentRepo } = useContext(DIContext);

    const [bookingStatus, book] = useApiCall(() =>
        appointmentRepo.book(appointment.id)
            .sideEffect(_result => {
                appointmentRepo.getById(appointment.id)
                    .sideEffect(newAppointment => {
                        dataDispatch({ type: 'updateAppointment', appointment: newAppointment });
                    })
            })
    , [appointment]);

    const [unBookingStatus, unBook] = useApiCall(() =>
        appointmentRepo.unBook(appointment.id).sideEffect(_result => {
                appointmentRepo.getById(appointment.id).sideEffect(newAppointment => {
                      dataDispatch({ type: 'updateAppointment', appointment: newAppointment });
                })
            })
    , [appointment]);

    useEffect(() => {
        [bookingStatus, unBookingStatus].map(x => {
            if(x instanceof Failed) {
                notificationDispatch({ type: 'addError', message: `Hiba foglalás/lemondás közben: ${x.error}` });
            }
        })
    }, [bookingStatus, unBookingStatus]);

    const isAttendee = () => loginState instanceof LoggedIn 
        && appointment.attendees.some(user => user.userName === loginState.user.userName);

    if(bookingStatus instanceof Loading) return <span>Booking...</span>;
    if(unBookingStatus instanceof Loading) return <span>Unbooking...</span>;
    
    if(loginState instanceof LoggedOut)
        return null;
        
    if(isAttendee())
        return <button onClick={() => unBook()}>Lemondás</button>;
    
    if(appointment.maxAttendees > appointment.attendees.length)
        return <button onClick={() => book()}>Foglalás</button>;
    
    return null;
}

const DeleteButton = ({ appointment }: { appointment: Appointment }) => {
    const { dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);

    const [deleteStatus, deleteAppointment] = useApiCall(() =>
        appointmentRepo.delete(appointment.id)
            .sideEffect(_ => {
                dataDispatch({ type: 'deleteAppointment', id: appointment.id });
            })
    , [appointment]);

    useEffect(() => {
        if(deleteStatus instanceof Failed) {
            console.error(deleteStatus.error);
        }
    }, [deleteStatus]);

    if(deleteStatus instanceof Loading) return <span>Deleting...</span>;

    return (
        <button onClick={() => deleteAppointment()}>Delete</button>
    );
}

export const AppointmentCardEditable = ({ appointment, onEdit, onView }: { appointment: Appointment, onEdit: (_: Appointment) => void, onView: (_: Appointment) => void }) => {
    const { loginState } = useContext(LoginContext);

    const isOwner = () => loginState instanceof LoggedIn
        && appointment.category.owner.userName === loginState.user.userName;

    return (
        <div className="appointmentCard">
            <a className="appointment-header clickable" onClick={() => onView(appointment)}>{appointment.category.name}</a>
            <div className="appointment-description">
                <p>{appointment.category.description}</p>
                <p>{appointment.category.price} Ft - {appointment.maxAttendees - appointment.attendees.length} szabad hely</p>
            </div>
            <div className="appointment-methods">
                <BookButton appointment={appointment} />
                {isOwner() &&
                <>
                    <DeleteButton appointment={appointment} />
                    <button onClick={() => onEdit(appointment)}>Edit</button>
                </>
                }
            </div>
        </div>
    );
}

export const AppointmentCard = ({ appointment }: { appointment: Appointment }) => {
    return (
        <div className="appointmentCard">
            <span className="appointment-header">{appointment.category.name}</span>
            <div className="appointment-description">
                <p>{appointment.category.description}</p>
                <p>{appointment.category.price} Ft - {appointment.maxAttendees - appointment.attendees.length} szabad hely</p>
            </div>
            <div className="appointment-methods">
                <BookButton appointment={appointment} />
            </div>
        </div>
    );
};