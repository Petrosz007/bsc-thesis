import React, { useCallback, useContext, useEffect, useState } from "react";
import { useApiCall, Failed, Loaded } from "../../hooks/apiCallHooks";
import { useHandleChange } from "../../hooks/useEditorForm";
import { AppointmentDTO } from "../../logic/dtos";
import { Appointment, Category, User } from "../../logic/entities";
import {DataAction, DataContext} from "../contexts/DataProvider";
import { DIContext } from "../contexts/DIContext";
import UserAdder from "./UserAdder";

import './AppointmentEditor.scss';
import { NotificationContext } from "../contexts/NotificationProvider";
import { ResultPromise } from "../../utilities/result";
import {DateTime} from "luxon";
import {EditorBase} from "./EditorBase";
import Select from "react-select";

interface AppointmentEditdata {
    id: number;
    startTimeDate: string;
    startTimeTime: string;
    endTimeDate: string;
    endTimeTime: string;
    categoryId: number;
    maxAttendees: number;

    createAnother: boolean;
}

const AppointmentEditorBase = ({ initialAppointment, apiCall, categories, onClose, labels }: {
    initialAppointment: Appointment,
    apiCall: (_x: AppointmentDTO) => ResultPromise<Appointment,Error>,
    categories: Category[],
    onClose: () => void,
    labels: {
      createAnother: string,
      submit: string,
    },
}) => {
    const initialAppointmentEditorState: AppointmentEditdata = {
        ...initialAppointment,
        startTimeDate: initialAppointment.startTime.toISODate(),
        startTimeTime: initialAppointment.startTime.toFormat('HH:mm'),
        endTimeDate: initialAppointment.endTime.toISODate(),
        endTimeTime: initialAppointment.endTime.toFormat('HH:mm'),
        categoryId: initialAppointment.category.id,
        createAnother: false,
    };
    
    const [users, setUsers] = useState<User[]>(initialAppointment.attendees);
    const [state, setState] = useState(initialAppointmentEditorState);

    const handleChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.type === 'checkbox' 
            ? event.target.checked
            : event.target.value;

        if(event.target.name === 'startTimeDate') {
            setState(prevState => ({
                ...prevState,
                endTimeDate: event.target.value,
            }))
        }

        setState(prevState => ({
            ...prevState,
            [event.target.name]: value,
        }));
    }, [setState]);
    
    const editorStateToDto = useCallback((editData: AppointmentEditdata) => ({
        ...editData,
        startTime: DateTime.fromISO(`${editData.startTimeDate}T${editData.startTimeTime}`).toUTC().toISO(),
        endTime: DateTime.fromISO(`${editData.endTimeDate}T${editData.endTimeTime}`).toUTC().toISO(),
        attendeeUserNames: users.map(user => user.userName),
    }), [users, state]);

    const dataDispatchAction = useCallback((appointment: Appointment) => ({ type: 'updateAppointment', appointment } as DataAction), []);
    
    const allowedUsers = useCallback((): User[]|undefined => {
        if(initialAppointment === undefined) return undefined;
        if(initialAppointment.category.everyoneAllowed) return undefined;

        return [...initialAppointment.category.allowedUsers, initialAppointment.category.owner];
    }, [initialAppointment]);
    
    const selectOptions = categories.map(c => ({ value: c.id, label: c.name }));

    return (
        <EditorBase
            state={state}
            apiCall={apiCall}
            handleChange={handleChange}
            editorStateToDto={editorStateToDto}
            labels={labels}
            onClose={onClose}
            dataDispatchAction={dataDispatchAction}
         >
            <label htmlFor="startTimeDate">Start</label>
            <div>
                <input type="date" name="startTimeDate" value={state.startTimeDate} onChange={handleChange} />
                <input type="time" name="startTimeTime" value={state.startTimeTime} onChange={handleChange} />
            </div>
            <label htmlFor="startTimeDate">End</label>
            <div>
                <input type="date" name="endTimeDate" value={state.endTimeDate} min={state.startTimeDate} onChange={handleChange} />
                <input type="time" name="endTimeTime" value={state.endTimeTime} onChange={handleChange} />
            </div>
            <label htmlFor="categoryId">Category</label>
            <Select options={selectOptions}
                    value={selectOptions.find(x => x.value === state.categoryId)}
                    onChange={x => {
                        if(x === null) return;
                        setState({ ...state, categoryId: x.value });
                    }} 
            />
            <label htmlFor="maxAttendees">MaxAttendees</label>
            <input type="number" name="maxAttendees" value={state.maxAttendees} min={Math.max(1, users.length)} onChange={handleChange} /><br/>
            <label htmlFor="attendees">Attendees</label>
            <UserAdder users={users} setUsers={setUsers} allowedUsers={allowedUsers()} max={state.maxAttendees} />
        </EditorBase>
    );
}

export const AppointmentEditorCreate = ({ categories, onClose }: {
    categories: Category[],
    onClose: () => void,
}) => {
    const { appointmentRepo } = useContext(DIContext);
    const initialState: Appointment = {
        id: 0,
        startTime: DateTime.now(),
        endTime: DateTime.now().plus({ hours: 1 }),
        category: categories[0],
        attendees: [],
        maxAttendees: 1,
    };
    
    return (
        <AppointmentEditorBase
            initialAppointment={initialState}
            apiCall={appointmentRepo.create}
            categories={categories}
            onClose={onClose}
            labels={{ createAnother: 'Több létrehozása', submit: 'Létrehozás' }}
        />
    );
}

export const AppointmentEditorUpdate = ({ appointment, categories, onClose }: {
    appointment: Appointment,
    categories: Category[],
    onClose: () => void,
}) => {
    const { appointmentRepo } = useContext(DIContext);

    const update = useCallback((dto: AppointmentDTO) =>
        appointmentRepo.update(dto)
            .andThen(_ => appointmentRepo.getById(dto.id))
    , [appointmentRepo]);

    return (
        <AppointmentEditorBase
            apiCall={update}
            initialAppointment={appointment}
            categories={categories}
            onClose={onClose}
            labels={{ createAnother: 'Maradok szerkeszteni', submit: 'Mentés' }}
        />
    );
}
