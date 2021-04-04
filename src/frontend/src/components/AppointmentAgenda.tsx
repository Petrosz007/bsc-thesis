import React, { ReactElement, useState } from "react";
import { Appointment, Category } from "../logic/entities";
import {Dictionary, groupBy, uniques} from "../utilities/listExtensions";
import {AppointmentCard, AppointmentCardEditable} from "./AppointmentCard";
import Select from 'react-select';

import './AppointmentAgenda.scss';
import Modal from "./Modal";
import { AppointmentEditorUpdate } from "./editors/AppointmentEditor";

const AppointmentAgendaBase = ({ appointments, categories, editable, showFull }: { 
    appointments: Appointment[],
    categories?: Category[],
    editable: boolean,
    showFull: boolean
}) => {
    const [startDate, setStartDate] = useState(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
    const [endDate, setEndDate] = useState(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0));
    
    const selectableCategories = uniques(appointments.map(a => a.category), c => `${c.id}`);
    const [selectedCategories, setSelectedCategories] = useState(selectableCategories);
    
    const sortedAppointments = [...appointments]
        .filter(a => selectedCategories.some(category => a.category.id === category.id))
        .filter(a => a.startTime >= startDate && a.startTime <= endDate)
        .filter(a => showFull || a.maxAttendees > a.attendees.length)
        .sort((left, right) => left.startTime.getTime() - right.startTime.getTime());
    
    const dictionary = groupBy(sortedAppointments, a => a.startTime.toDateString());

    const [appointmentToEdit, setAppointmentToEdit] = useState<Appointment|undefined>(undefined);

    return (
        <div>
            {appointmentToEdit !== undefined && categories !== undefined &&
                <Modal>
                    <AppointmentEditorUpdate appointment={appointmentToEdit} categories={categories} onClose={() => setAppointmentToEdit(undefined)}  />
                </Modal>
            }
            Categories:
            <Select options={selectableCategories.map(c => ({ value: c, label: c.name }))}
                    onChange={e => {
                        const arr = Array.isArray(e) ? e : [];
                        setSelectedCategories(arr.length !== 0 ? e.map(x => x.value) : selectableCategories);
                    }}
                    isMulti
            />
            Start:
            <input type="date"
                   value={startDate.toISOString().slice(0,10)}
                   onChange={e => setStartDate(new Date(e.target.value))}
                   max={endDate.toISOString().slice(0,10)}
            />
            End:
            <input type="date"
                   value={endDate.toISOString().slice(0,10)}
                   onChange={e => setEndDate(new Date(e.target.value))}
                   min={startDate.toISOString().slice(0,10)}
            />
            <table className="agenda-table">
                <tbody>
                {Dictionary.keys(dictionary).map(key =>
                    dictionary[key].map((appointment, index) =>
                        <tr key={appointment.id}>
                            {index === 0 && <td rowSpan={dictionary[key].length} className="agenda-date">{key}</td>}
                            <td className="agenda-time">{appointment.startTime.toTimeString().slice(0,5)} - {appointment.endTime.toTimeString().slice(0,5)}</td>
                            <td className="agenda-detail">
                                {editable
                                    ? <AppointmentCardEditable appointment={appointment} onEdit={a => setAppointmentToEdit(a)}/>
                                    : <AppointmentCard appointment={appointment} />
                                }
                            </td>
                        </tr>
                    )
                )}
                </tbody>
            </table>
        </div>
    );
}

export const AppointmentAgenda = ({ appointments, showFull = true }: { appointments: Appointment[], showFull?: boolean }) => {
    return (
        <AppointmentAgendaBase appointments={appointments} editable={false} showFull={showFull} />
    );
}

export const AppointmentAgendaEditable = ({ appointments, categories, showFull = true }: { appointments: Appointment[], categories: Category[], showFull?: boolean }) => {
    return (
        <AppointmentAgendaBase appointments={appointments} categories={categories} editable={true} showFull={showFull} />
    );
}
