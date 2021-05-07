import React, {useCallback, useContext, useMemo, useState} from "react";
import { AppointmentDTO } from "../../logic/dtos";
import { Appointment, Category, User } from "../../logic/entities";
import {DataAction} from "../contexts/DataProvider";
import UserAdder from "./UserAdder";
import { ResultPromise } from "../../utilities/result";
import {DateTime} from "luxon";
import {EditorBase} from "./EditorBase";
import Select from "react-select";
import {DateTimePicker} from "../inputs/DatePicker";
import {NotificationContext} from "../contexts/NotificationProvider";
import {DIContext} from "../contexts/DIContext";

interface AppointmentEditdata {
    id: number;
    startTime: DateTime;
    endTime: DateTime;
    category: Category;
    maxAttendees: number;

    createAnother: boolean;
}

const AppointmentEditorBase = ({ initialAppointment, apiCall, categories, onClose, labels }: {
    initialAppointment: Appointment,
    apiCall: (_x: AppointmentDTO) => ResultPromise<Appointment,Error>,
    categories: Category[],
    onClose: () => void,
    labels: { 
        header: string,
        createAnother: string,
        submit: string,
    },
}) => {
    const { notificationDispatch } = useContext(NotificationContext);
    const initialAppointmentEditorState: AppointmentEditdata = {
        ...initialAppointment,
        startTime: initialAppointment.startTime,
        endTime: initialAppointment.endTime,
        category: initialAppointment.category,
        maxAttendees: initialAppointment.category.maxAttendees,
        createAnother: false,
    };
    
    const [users, setUsers] = useState<User[]>(initialAppointment.attendees);
    const [state, setState] = useState(initialAppointmentEditorState);

    const handleChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.type === 'checkbox' 
            ? event.target.checked
            : event.target.value;

        setState(prevState => ({
            ...prevState,
            [event.target.name]: value,
        }));
    }, [setState]);
    
    const editorStateToDto = useCallback((editData: AppointmentEditdata): AppointmentDTO => ({
        ...editData,
        categoryId: editData.category.id,
        startTime: editData.startTime.toUTC().toISO(),
        endTime: editData.endTime.toUTC().toISO(),
        attendeeUserNames: users.map(user => user.userName),
    }), [users]);

    const dataDispatchAction = useCallback((appointment: Appointment) => ({ type: 'updateAppointment', appointment } as DataAction), []);
    
    const allowedUsers = useMemo((): User[]|undefined => {
        if(state === undefined) return undefined;
        if(state.category.everyoneAllowed) return undefined;

        return [...state.category.allowedUsers, state.category.owner];
    }, [state]);
    
    const validator = useCallback((editData: AppointmentEditdata) => {
        const allAllowed = users.map(user => {
            if(allowedUsers === undefined || allowedUsers.some(u => u.userName === user.userName)) {
                return true;
            }
            notificationDispatch({ type: 'addError', message: `${user.name} nem engedélyezett résztvevő a kategórián. Szekeszd a kategóriát, ha hozzá szeretnéd adni.` });
            return false;
        });
        
        if(!allAllowed.every(x => x)) return false;
        
        return true;
    }, [users, allowedUsers, notificationDispatch]);
    
    const selectOptions = useMemo(() => categories.map(c => ({ value: c, label: c.name })), [categories]);

    return (
        <EditorBase
            state={state}
            apiCall={apiCall}
            handleChange={handleChange}
            editorStateToDto={editorStateToDto}
            labels={labels}
            onClose={onClose}
            dataDispatchAction={dataDispatchAction}
            validator={validator}
         >
            <div className="editorGroup">
                <label htmlFor="startTimeDate">Kezdés</label>
                <div>
                    <DateTimePicker valueDate={state.startTime} 
                                    onChangeDate={x => setState(prevState => ({ 
                                        ...prevState, 
                                        startTime: x,
                                        endTime: prevState.endTime.set({ year: x.year, month: x.month, day: x.day })
                                    }))}
                    />
                </div>
            </div>
            <div className="editorGroup">
                <label htmlFor="startTimeDate">Vége</label>
                <div>
                    <DateTimePicker valueDate={state.endTime}
                                    onChangeDate={x => setState(prevState => ({ ...prevState, endTime: x }))}
                                    minDate={state.startTime}
                    />
                </div>
            </div>
            <div className="editorGroup">
                <label htmlFor="categoryId">Kategória</label>
                <Select options={selectOptions}
                        value={selectOptions.find(x => x.value.id === state.category.id)}
                        onChange={x => {
                            if(x === null) return;
                            setState({ ...state, category: x.value, maxAttendees: x.value.maxAttendees });
                        }} 
                />
            </div>
            <div className="editorGroup">
                <label htmlFor="maxAttendees">Max résztvevők</label>
                <input type="number" name="maxAttendees" value={state.maxAttendees} min={Math.max(1, users.length)} onChange={handleChange} />
            </div>
            <div className="editorGroup">
                <label htmlFor="attendees">Résztvevők</label>
                <UserAdder users={users} setUsers={setUsers} allowedUsers={allowedUsers} max={state.maxAttendees} />
            </div>
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
            labels={{ header: 'Új Időpont', createAnother: 'Több létrehozása', submit: 'Létrehozás' }}
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
            labels={{ header: 'Időpont Szerkesztése',createAnother: 'Maradok szerkeszteni', submit: 'Mentés' }}
        />
    );
}
