import React from "react";
import {Appointment} from "../logic/entities";

import './AppointmentViewer.scss';
import UserName from "./UserName";
import {DateTime} from "luxon";

export default ({ appointment, onClose }: { appointment: Appointment, onClose: () => void }) => {
    return (
        <div className="appointment-viewer">
            <div className="viewer-content">
                <span className="appointment-header">{appointment.category.name}</span>
                <p>{appointment.category.description}</p>
                <p>{appointment.category.price} Ft</p>
                <p>
                    {appointment.startTime.toLocaleString(DateTime.DATETIME_FULL)}<br/>
                    {appointment.endTime.toLocaleString(DateTime.DATETIME_FULL)}
                </p>
                <p>Max résztvevők: {appointment.maxAttendees}</p>
                <p>Szabad hely: {appointment.maxAttendees - appointment.attendees.length}</p>
                <p>Jelenlegi résztvevők: {appointment.attendees.length} fő</p>
                <ul>
                    {appointment.attendees.map(attendee => 
                        <li key={attendee.userName}><UserName user={attendee} /></li>
                    )}
                </ul>
            </div>
            <div className="viewer-footer">
                <button className="viewer-footer" onClick={e => {e.preventDefault(); onClose();}}>Mégse</button>
            </div>
        </div>
    );
}