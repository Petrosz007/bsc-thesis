import React, { ReactElement } from "react";
import { Appointment } from "../logic/entities";
import { Dictionary, groupBy } from "../utilities/listExtensions";
import AppointmentCard from "./AppointmentCard";

import './AppointmentAgenda.scss';

export default ({ appointments }: { appointments: Appointment[] }) => {
    const sorted = [...appointments].sort((left, right) => left.startTime.getTime() - right.startTime.getTime());
    const dictionary = groupBy(sorted, a => a.startTime.toDateString());

    return (
        <table className="agenda-table">
            <tbody>
            {Dictionary.keys(dictionary).map(key =>
                dictionary[key].map((appointment, index) =>
                    <tr key={appointment.id}>
                        {index === 0 && <td rowSpan={dictionary[key].length} className="agenda-date">{key}</td>}
                        <td className="agenda-time">{appointment.startTime.toTimeString().slice(0,5)} - {appointment.endTime.toTimeString().slice(0,5)}</td>
                        <td className="agenda-detail"><AppointmentCard appointment={appointment} /></td>
                    </tr>
                )
            )}
            </tbody>
        </table>
    );
}