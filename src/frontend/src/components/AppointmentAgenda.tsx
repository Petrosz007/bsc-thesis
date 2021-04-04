import React, { ReactElement, useState } from "react";
import { Appointment, Category } from "../logic/entities";
import {Dictionary, groupBy, uniques} from "../utilities/listExtensions";
import {AppointmentCard, AppointmentCardEditable} from "./AppointmentCard";
import Select from 'react-select';

import './AppointmentAgenda.scss';
import Modal from "./Modal";
import { AppointmentEditorUpdate } from "./editors/AppointmentEditor";
import {DateTime, Duration, Interval} from "luxon";
import {DatePicker, DateRangePicker} from "./inputs/DatePicker";

const AppointmentAgendaBase = ({ appointments, categories, editable, showFull }: { 
    appointments: Appointment[],
    categories?: Category[],
    editable: boolean,
    showFull: boolean
}) => {
    const startOfTheMonth = DateTime.now().set({ day: 1, hour: 0, minute: 0, second: 0, millisecond: 0 });
    const endOfTheMonth = DateTime.now().set({ day: DateTime.now().daysInMonth, hour: 23, minute: 59, second: 59, millisecond: 59 });
    
    const [dateInterval, setDateInterval] = useState(Interval.fromDateTimes(startOfTheMonth, endOfTheMonth));
    
    const selectableCategories = uniques(appointments.map(a => a.category), c => `${c.id}`);
    const [selectedCategories, setSelectedCategories] = useState(selectableCategories);
    
    const sortedAppointments = [...appointments]
        .filter(a => selectedCategories.some(category => a.category.id === category.id))
        .filter(a => dateInterval.contains(a.startTime))
        .filter(a => showFull || a.maxAttendees > a.attendees.length)
        .sort((left, right) => left.startTime.toMillis() - right.startTime.toMillis());
    
    const dictionary = groupBy(sortedAppointments, a => a.startTime.setLocale('hu').toLocaleString(DateTime.DATE_MED_WITH_WEEKDAY));

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
            <DateRangePicker value={dateInterval} onChange={setDateInterval} />
            <table className="agenda-table">
                <tbody>
                {Dictionary.keys(dictionary).map(key =>
                    dictionary[key].map((appointment, index) =>
                        <tr key={appointment.id}>
                            {index === 0 && <td rowSpan={dictionary[key].length} className="agenda-date">{key}</td>}
                            <td className="agenda-time">{appointment.startTime.toLocaleString(DateTime.TIME_24_SIMPLE)} - {appointment.endTime.toLocaleString(DateTime.TIME_24_SIMPLE)}</td>
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
