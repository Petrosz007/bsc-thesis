import React, { useContext, useEffect, useMemo } from 'react';
import { Appointment } from '../logic/entities';
import {LoggedIn, LoggedOut, LoginContext} from './contexts/LoginProvider';
import { Failed, Loading, useApiCall } from '../hooks/apiCallHooks';
import { DIContext } from './contexts/DIContext';
import { DataContext } from './contexts/DataProvider';

import './AppointmentCard.scss';
import { NotificationContext } from './contexts/NotificationProvider';
import {DateTime} from "luxon";
import {DeleteIcon, EditIcon, InfoIcon} from '../SVGs';

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

const BookButton = ({ appointment, className }: { appointment: Appointment, className?: string }) => {
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

    // if(bookingStatus instanceof Loading) return <span>Foglalás...</span>;
    // if(unBookingStatus instanceof Loading) return <span>Lemondás...</span>;
    
    if(loginState instanceof LoggedOut)
        return null;
        
    if(isAttendee())
        return <button className={`unbook ${className}`} onClick={() => {
            if(window.confirm('Biztos le szeretnéd mondani ezt az időpontot?')) 
                unBook();
        }}>Lemondás</button>;
    
    if(appointment.maxAttendees > appointment.attendees.length)
        return <button className={`book ${className}`} onClick={() => book()}>Foglalás</button>;
    
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

    // if(deleteStatus instanceof Loading) return <span>Törlés...</span>;

    return (
        <button onClick={() => {
            if(window.confirm('Biztos ki szeretnéd törölni ezt az időpontot?'))
                deleteAppointment();
        }}><DeleteIcon className="deleteIcon" /></button>
    );
}

const AppointmentCardBase = ({ editable, appointment }: {
    editable?: {
        onEdit: (_: Appointment) => void,
        onView: (_: Appointment) => void,
    },
    appointment: Appointment,
}) => {
    const { loginState } = useContext(LoginContext);

    const isOwner = () => loginState instanceof LoggedIn
        && appointment.category.owner.userName === loginState.user.userName;
    
    const freeSlots = useMemo(() => appointment.maxAttendees - appointment.attendees.length, [appointment]);

    return (
        <div className={`appointmentCard ${editable !== undefined ? 'editable' : ''}`}>
            <div className="appointmentTime">
                {appointment.startTime.hasSame(appointment.endTime, 'day')
                    ? <>
                    <p>{appointment.startTime.toFormat('HH:mm')}</p>
                    <p>{appointment.endTime.toFormat('HH:mm')}</p>
                    </>
                    : <>
                    <p>{appointment.startTime.toFormat('HH:mm')}</p>
                    <p>
                        {appointment.startTime.hasSame(appointment.endTime, 'year') || 
                            <>{appointment.endTime.toFormat('yyyy')}<br/></>}
                        {appointment.endTime.toFormat('MM.dd')}<br/>
                        {appointment.endTime.toFormat('HH:mm')}
                    </p>
                    </>}
            </div>
            <p className="appointment-header">{appointment.category.name}<span>{freeSlots === 0 ? 'Nincs' : freeSlots} szabad hely</span></p>
            <div className="appointment-description">
                <p>{appointment.category.description}</p>
                <p>{appointment.category.price} Ft </p>
            </div>
            <BookButton className="bookButton" appointment={appointment} />
            <div className="appointment-methods">
                {isOwner() && editable !== undefined &&
                <>
                    <button onClick={() => editable?.onView(appointment)}><InfoIcon className="infoIcon"/></button>
                    <button onClick={() => editable.onEdit(appointment)}><EditIcon className="editIcon"/></button>
                    <DeleteButton appointment={appointment} />
                </>
                }
            </div>
        </div>
    );
};

export const AppointmentCardEditable = ({ appointment, onEdit, onView }: { 
    appointment: Appointment,
    onEdit: (_: Appointment) => void,
    onView: (_: Appointment) => void
}) => <AppointmentCardBase editable={{ onEdit, onView }} appointment={appointment} />;

export const AppointmentCard = ({ appointment }: { appointment: Appointment }) =>
    <AppointmentCardBase appointment={appointment} />;
