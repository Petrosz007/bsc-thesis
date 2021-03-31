import React, { ReactElement, useState } from "react";
import { Appointment, Category } from "../logic/entities";
import { Dictionary, groupBy } from "../utilities/listExtensions";
import AppointmentCard from "./AppointmentCard";

import './AppointmentAgenda.scss';
import Modal from "./Modal";
import { AppointmentEditorUpdate } from "./editors/AppointmentEditor";

export default ({ appointments, categories }: { appointments: Appointment[], categories? : Category[] }) => {
    const sorted = [...appointments].sort((left, right) => left.startTime.getTime() - right.startTime.getTime());
    const dictionary = groupBy(sorted, a => a.startTime.toDateString());

    const [appointmentToEdit, setappointmentToEdit] = useState<Appointment|undefined>(undefined);

    return (
        <>
        {appointmentToEdit !== undefined && categories !== undefined &&
            <Modal>
                <AppointmentEditorUpdate appointment={appointmentToEdit} categories={categories} onClose={() => setappointmentToEdit(undefined)}  />
            </Modal>
        }
        <table className="agenda-table">
            <tbody>
            {Dictionary.keys(dictionary).map(key =>
                dictionary[key].map((appointment, index) =>
                    <tr key={appointment.id}>
                        {index === 0 && <td rowSpan={dictionary[key].length} className="agenda-date">{key}</td>}
                        <td className="agenda-time">{appointment.startTime.toTimeString().slice(0,5)} - {appointment.endTime.toTimeString().slice(0,5)}</td>
                        <td className="agenda-detail"><AppointmentCard appointment={appointment} onEdit={a => setappointmentToEdit(a)}/></td>
                    </tr>
                )
            )}
            </tbody>
        </table>
        </>
    );
}