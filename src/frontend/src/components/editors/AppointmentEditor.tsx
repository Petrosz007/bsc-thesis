import React, { useContext, useEffect, useState } from "react";
import { useApiCall, Failed, Loaded } from "../../hooks/apiCallHooks";
import { useHandleChange } from "../../hooks/useEditorForm";
import { AppointmentDTO } from "../../logic/dtos";
import { Category, User } from "../../logic/entities";
import { DataContext } from "../contexts/DataProvider";
import { DIContext } from "../contexts/DIContext";
import UserAdder from "./UserAdder";

import './AppointmentEditor.scss';
import { NotificationContext } from "../contexts/NotificationProvider";

interface AppointmentEditdata {
    startTimeDate: string;
    startTimeTime: string;
    endTimeDate: string;
    endTimeTime: string;
    categoryId: number;
    maxAttendees: number;

    createAnother: boolean;
}

export default ({ categories, onClose }: {
    categories: Category[],
    onClose: () => void,
}) => {
    const initialAppointmentEditorState: AppointmentEditdata = {
        startTimeDate: new Date().toISOString().slice(0,10),
        startTimeTime: new Date().toISOString().slice(11,16),
        endTimeDate: new Date(Date.now() + 60*60*1000).toISOString().slice(0,10),
        endTimeTime: new Date(Date.now() + 60*60*1000).toISOString().slice(11,16),
        categoryId: categories[0].id,
        maxAttendees: 1,
        createAnother: false,
    };

    const { dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);

    const [users, setUsers] = useState<User[]>([]);
    const [closeAfterLoad, setCloseAfterLoad] = useState(false);
    const [state, setState] = useState(initialAppointmentEditorState);

    const [createAppointmentState, createAppointment] = useApiCall((dto: AppointmentDTO) => 
        appointmentRepo.create(dto)
            .sideEffect(appointment => {
                console.log('Created', appointment);
                dataDispatch({ type: 'updateAppointment', appointment });
            })
    , []);

    const handleChange = useHandleChange(setState);

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        setCloseAfterLoad(!state.createAnother);
        createAppointment({ 
            ...state,
            startTime: new Date(`${state.startTimeDate} ${state.startTimeTime}`).toISOString(),
            endTime: new Date(`${state.endTimeDate} ${state.endTimeTime}`).toISOString(),
            attendeeUserNames: users.map(user => user.userName),
            id: 0,
        }, !state.createAnother);

        event.preventDefault();
    }

    useEffect(() => {
        if(createAppointmentState instanceof Failed) {
            console.error('Error in AppointmentEditor: ', createAppointmentState.error);
        }
        if(createAppointmentState instanceof Loaded){
            if(closeAfterLoad) {
                onClose();
            } else {
                setState(prevState => ({
                    ...initialAppointmentEditorState,
                    createAnother: prevState.createAnother,
                }));
                setUsers([]);
            }
        }
    }, [createAppointmentState, closeAfterLoad]);

    return (
        <>
        {createAppointmentState instanceof Failed && <p>Error while creating appointment: {createAppointmentState.error.message}</p>}
        <form onSubmit={handleSubmit} className="appointment-editor-form">
            <div className="editor-inputs">
                <label htmlFor="startTimeDate">Start</label>
                <div>
                    <input type="date" name="startTimeDate" value={state.startTimeDate} onChange={handleChange} />
                    <input type="time" name="startTimeTime" value={state.startTimeTime} onChange={handleChange} />
                </div>
                <label htmlFor="startTimeDate">End</label>
                <div>
                    <input type="date" name="endTimeDate" value={state.endTimeDate} onChange={handleChange} />
                    <input type="time" name="endTimeTime" value={state.endTimeTime} onChange={handleChange} />
                </div>
                <label htmlFor="categoryId">Category</label>
                <select name="categoryId" value={state.categoryId} onChange={handleChange}>
                    {categories.map(category => 
                        <option value={category.id} key={category.id}>{category.name}</option>
                        )}
                </select>
                <label htmlFor="maxAttendees">MaxAttendees</label>
                <input type="number" name="maxAttendees" value={state.maxAttendees} onChange={handleChange} /><br/>
            </div>

            <div className="editor-user-adder">
                <label htmlFor="attendees">Attendees</label>
                <UserAdder users={users} setUsers={setUsers} />
            </div>

            <div className="editor-footer">
                <div className="editor-footer-checkbox">
                    <label htmlFor="createAnother">Create another</label>
                    <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
                </div>
                <input className="editor-footer-submit" type="submit" value="Submit"/>
                <button onClick={e => {e.preventDefault(); onClose();}}>Cancel</button>
            </div>
        </form>
        </>
    );
}