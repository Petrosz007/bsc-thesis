import React from 'react';
import { Appointment } from 'src/logic/entities';
import CategoryCard from './CategoryCard';
import UserCard from './UserCard';

import './AppointmentCard.scss';

export default ({ appointment }: { appointment: Appointment }) => {

    return (
        <div className="appointment-card">
            <p>Id: {appointment.id}</p>
            <p>Start: {appointment.startTime}</p>
            <p>End: {appointment.endTime}</p>
            Caegory: <CategoryCard category={appointment.category} />
            Attendees: <ul>
                {appointment.attendees.map(attendee => 
                    <UserCard user={attendee}/>
                )}
            </ul>
            <p>Max Attendees{appointment.maxAttendees}</p>
        </div>
    );
};