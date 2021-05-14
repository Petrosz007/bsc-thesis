import React, {useEffect, useMemo, useState} from "react";
import { Appointment, Category } from "../logic/entities";
import {Dictionary, groupBy, uniques} from "../utilities/listExtensions";
import {AppointmentCard, AppointmentCardEditable} from "./AppointmentCard";
import Select from 'react-select';

import './AppointmentAgenda.scss';
import Modal from "./Modal";
import { AppointmentEditorUpdate } from "./editors/AppointmentEditor";
import {DateTime, Interval} from "luxon";
import {DateRangePicker} from "./inputs/DatePicker";
import AppointmentViewer from "./AppointmentViewer";

const AppointmentAgendaBase = ({ appointments, categories, editable, showFull }: { 
    appointments: Appointment[],
    categories?: Category[],
    editable: boolean,
    showFull: boolean
}) => {
    const [dateInterval, setDateInterval] = useState(() => {
        const startOfTheMonth = DateTime.now().set({ day: 1, hour: 0, minute: 0, second: 0, millisecond: 0 });
        const endOfTheMonth = DateTime.now().set({ day: DateTime.now().daysInMonth, hour: 23, minute: 59, second: 59, millisecond: 59 });
        return Interval.fromDateTimes(startOfTheMonth, endOfTheMonth);
    });
    const [selectedCategories, setSelectedCategories] = useState<Category[]>([]);
    const [appointmentToEdit, setAppointmentToEdit] = useState<Appointment|undefined>(undefined);
    const [appointmentToView, setAppointmentToView] = useState<Appointment|undefined>(undefined);
    
    const selectableCategories = useMemo(() => 
        categories
            ?? uniques(appointments.map(a => a.category), c => `${c.id}`)
    , [appointments, categories]);
    
    // If there is a new category to be selected, only change the selected category, when there is none selected
    // This way the filter doesn't get overwritten
    useEffect(() => {
        setSelectedCategories(prevSelectedCategories =>
            prevSelectedCategories.length === 0
                ? []
                : prevSelectedCategories
        );
    }, [selectableCategories]);
    
    const dictionary = useMemo(() => {
        const sorted = [...appointments]
            .filter(a => selectedCategories.length === 0 ||  selectedCategories.some(category => a.category.id === category.id))
            .filter(a => dateInterval.contains(a.startTime))
            .filter(a => showFull || a.maxAttendees > a.attendees.length)
            .sort((left, right) => (left.startTime.toMillis() - right.startTime.toMillis()) * 10000 + (right.id - left.id));
        return groupBy(sorted, a => a.startTime.toFormat('yyyy.MM.dd, cccc'));
    }, [appointments, selectedCategories, dateInterval, showFull]);

    return (
        <div className="appointmentAgenda">
            {appointmentToEdit !== undefined && categories !== undefined &&
                <Modal>
                    <AppointmentEditorUpdate appointment={appointmentToEdit} categories={categories} onClose={() => setAppointmentToEdit(undefined)}  />
                </Modal>
            }
            {appointmentToView !== undefined &&
                <Modal>
                    <AppointmentViewer appointment={appointmentToView} onClose={() => setAppointmentToView(undefined)}  />
                </Modal>
            }
            <h2>Időpontok</h2>
            <div className="agendaContent">
                <div className="agendaFilters">
                    <p>Kategóriák:</p>
                    <Select options={selectableCategories.map(c => ({ value: c, label: c.name }))}
                            onChange={e => {
                                const arr = Array.isArray(e) ? e : [];
                                setSelectedCategories(arr.length !== 0 ? e.map(x => x.value) : selectableCategories);
                            }}
                            placeholder="Válassz kategóriákat..."
                            isMulti
                    />
                    <p>Intervallum:</p>
                    <DateRangePicker value={dateInterval} onChange={setDateInterval} />
                </div>
                <div className="agenda-table">
                    {Dictionary.keys(dictionary).length === 0
                        ? <p className="noAppointmentsFound">Nem található egy időpont sem.</p>
                        : Dictionary.keys(dictionary).map(key =>
                        <div className="agenda-table-day" key={key}>
                            <p className="agenda-date">{key}</p>
                            <div className="agendaDayCards">
                                {dictionary[key].map((appointment) =>
                                    <React.Fragment key={appointment.id}>
                                    {editable
                                        ? <AppointmentCardEditable appointment={appointment} onEdit={a => setAppointmentToEdit(a)} onView={a => setAppointmentToView(a)}/>
                                        : <AppointmentCard appointment={appointment} />}
                                    <hr/>
                                    </React.Fragment>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            </div>
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
