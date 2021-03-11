import React, { useContext } from 'react';
import { Appointment } from 'src/logic/entities';
import CategoryCard from './CategoryCard';
import UserCard from './UserCard';

import './AppointmentCard.scss';
import { LoggedIn, LoginContext } from './contexts/LoginProvider';
import { Loading, useApiCall } from '../hooks/apiCallHooks';
import { DIContext } from './contexts/DIContext';
import { DataContext } from './contexts/DataProvider';

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
    const month = padNum(date.getMonth());
    const day = padNum(date.getDay());
    const hour = padNum(date.getHours());
    const minute = padNum(date.getMinutes());
    return <>{year}.{month}.{day} {hour}:{minute}</>;
}

export default ({ appointment }: { appointment: Appointment }) => {
    const { loginState } = useContext(LoginContext);
    const { dataDispatch } = useContext(DataContext);
    
    const { appointmentRepo } = useContext(DIContext);

    const [bookingStatus, book] = useApiCall(async () => {
        await appointmentRepo.book(appointment.id);
        const newAppointment = await appointmentRepo.getById(appointment.id);
        dataDispatch({ type: 'updateAppointment', appointment: newAppointment });
    }, [appointment]);

    const [unBookingStatus, unBook] = useApiCall(async () => {
        await appointmentRepo.unBook(appointment.id);
        const newAppointment = await appointmentRepo.getById(appointment.id);
        dataDispatch({ type: 'updateAppointment', appointment: newAppointment });
    }, [appointment]);
    
    const isAttendee = () => loginState instanceof LoggedIn 
        && appointment.attendees.some(user => user.userName == loginState.user.userName);

    const button = () => {
        if(bookingStatus instanceof Loading) return <span>Booking...</span>;
        if(unBookingStatus instanceof Loading) return <span>Unbooking...</span>;
        
        return <>
            {bookingStatus instanceof Error && <span>Error booking</span>}
            {unBookingStatus instanceof Error && <span>Error unbooking</span>}
            {isAttendee()
                    ? <button onClick={() => unBook()}>Lemondás</button> 
                    : <button onClick={() => book()}>Foglalás</button>}
        </>;
    }

    return (
        <div className="appointmentCard">
            <div className="cardTop">
                <span>{appointment.category.name}</span>
            </div>
            <div className="cardBottom">
                <p>{appointment.category.description}</p>
                <p><FormattedDate date={appointment.startTime}/> - <HourDuration startTime={appointment.startTime} endTime={appointment.endTime}/></p>
                <p>{appointment.category.price} Ft</p>
                {button()}
            </div>
        </div>
    );
};