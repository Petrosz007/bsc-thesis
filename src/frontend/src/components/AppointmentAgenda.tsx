import React, { ReactElement, useState } from "react";
import { Appointment, Category } from "../logic/entities";
import {Dictionary, groupBy, uniques} from "../utilities/listExtensions";
import {AppointmentCard, AppointmentCardEditable} from "./AppointmentCard";

import './AppointmentAgenda.scss';
import Modal from "./Modal";
import { AppointmentEditorUpdate } from "./editors/AppointmentEditor";

const AppointmentAgendaBase = ({ appointments, categories, editable }: { appointments: Appointment[], categories?: Category[], editable: boolean }) => {
    const [startDate, setStartDate] = useState(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
    const [endDate, setEndDate] = useState(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0));
    
    const selectableCategories = uniques(appointments.map(a => a.category), c => `${c.id}`);
    const [selectedCategories, setSelectedCategories] = useState<string[]>(selectableCategories.map(c => `${c.id}`));
    
    const sortedAppointments = [...appointments]
        .filter(a => selectedCategories.some(categoryId => `${a.category.id}` === categoryId))
        .filter(a => a.startTime >= startDate && a.startTime <= endDate)
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
            <select value={selectedCategories}
                    onChange={e => setSelectedCategories(Array.from(e.target.selectedOptions, x => x.value))}
                    multiple={true}
            >
                {selectableCategories.map(category =>
                    <option value={`${category.id}`} key={category.id}>{category.name}</option>
                )}
            </select>
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

export const AppointmentAgenda = ({ appointments }: { appointments: Appointment[] }) => {
    return (
        <AppointmentAgendaBase appointments={appointments} editable={false} />
    );
}

export const AppointmentAgendaEditable = ({ appointments, categories }: { appointments: Appointment[], categories: Category[] }) => {
    return (
        <AppointmentAgendaBase appointments={appointments} categories={categories} editable={true} />
    );
}
