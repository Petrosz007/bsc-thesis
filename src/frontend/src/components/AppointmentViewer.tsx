import React, {useMemo} from "react";
import {Appointment} from "../logic/entities";
import UserName from "./UserName";
import {DateTime} from "luxon";

import './DetailViewer.scss';

export default ({ appointment, onClose }: { appointment: Appointment, onClose: () => void }) => {
    const freeSlots = useMemo(() => appointment.maxAttendees - appointment.attendees.length, [appointment]);
    
    return (
        <div className="detailViewer">
            <span className="viewer-header">{appointment.category.name}</span>
            <div className="viewer-content">
                <div>
                    <span>Kezdődik</span>
                    <p>{appointment.startTime.toLocaleString(DateTime.DATETIME_FULL)}</p>
                </div>
                <div>
                    <span>Végződik</span>
                    <p>{appointment.endTime.toLocaleString(DateTime.DATETIME_FULL)}</p>
                </div>
                <div>
                    <span>Max résztvevők</span>
                    <p>{appointment.maxAttendees} fő</p>
                </div>
                <div>
                    <span>Szabad hely</span>
                    <p>{freeSlots === 0 ? 'Nincs' : freeSlots} szabad hely</p>
                </div>
                <div>
                    <span>Résztvevők</span>
                    <p>{appointment.attendees.length} fő</p>
                    <ul className="allowedUsersList">
                        {appointment.attendees.map(attendee => 
                            <li key={attendee.userName}><UserName user={attendee} /></li>
                        )}
                    </ul>
                </div>
            </div>
            <div className="viewer-footer">
                <button className="viewer-footer" onClick={e => {e.preventDefault(); onClose();}}>Bezárás</button>
            </div>
        </div>
    );
}