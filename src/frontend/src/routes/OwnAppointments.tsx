import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Category, User } from "../logic/entities";
import React, { useCallback, useContext, useEffect, useState } from "react";
import { Redirect } from "react-router";
import { AppointmentDTO } from "../logic/dtos";
import AppointmentAgenda from "../components/AppointmentAgenda";
import UserAdder from "../components/UserAdder";

import './OwnAppointments.scss'
interface AppointmentEditdata {
    startTimeDate: string;
    startTimeTime: string;
    endTimeDate: string;
    endTimeTime: string;
    categoryId: number;
    maxAttendees: number;
}

const AppointmentEditor = ({ categories, onSubmit }: { categories: Category[], onSubmit: (_x: AppointmentDTO) => void }) => {
    const users = useContext(DataContext).dataState.users;

    const [state, setState] = useState<AppointmentEditdata>({
        startTimeDate: new Date().toISOString().slice(0,10),
        startTimeTime: new Date().toISOString().slice(11,16),
        endTimeDate: new Date(Date.now() + 60*60*1000).toISOString().slice(0,10),
        endTimeTime: new Date(Date.now() + 60*60*1000).toISOString().slice(11,16),
        categoryId: categories[0].id,
        maxAttendees: 1,
    });

    const handleChange = (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setState({
            ...state,
            [event.target.name]: event.target.value,
        });
    };

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        onSubmit({ 
            ...state,
            startTime: new Date(`${state.startTimeDate} ${state.startTimeTime}`).toISOString(),
            endTime: new Date(`${state.endTimeDate} ${state.endTimeTime}`).toISOString(),
            attendeeUserNames: users.map(user => user.userName),
            id: 0,
        });
        event.preventDefault();
    }

    return (
        <form onSubmit={handleSubmit}>
            {/* <input type="date" value={state.startTime}/> */}
            Start:
            <input type="date" name="startTimeDate" value={state.startTimeDate} onChange={handleChange} />
            <input type="time" name="startTimeTime" value={state.startTimeTime} onChange={handleChange} />
            <br/>
            End:
            <input type="date" name="endTimeDate" value={state.endTimeDate} onChange={handleChange} />
            <input type="time" name="endTimeTime" value={state.endTimeTime} onChange={handleChange} />
            <br/>
            Category:
            <select name="categoryId" value={state.categoryId} onChange={handleChange}>
                {categories.map(category => 
                    <option value={category.id} key={category.id}>{category.name}</option>
                )}
            </select><br/>
            MaxAttendees: <input type="number" name="maxAttendees" value={state.maxAttendees} onChange={handleChange} /><br/>
            <UserAdder />
            <input type="submit" value="Submit"/>
        </form>
    );
}

const OwnAppointments = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo, categoryRepo } = useContext(DIContext);

    const [state, refreshData] = useApiCall(() =>
        appointmentRepo.getContractorsAppointments(user.userName)
            .sideEffect(appointments => {
                dataDispatch({ type: 'setAppointments', appointments });
            })
            .andThen(_ => categoryRepo.getContractorsCategories(user.userName))
            .sideEffect(categories => {
                dataDispatch({ type: 'setCategories', categories });
            })
    , []);

    const [creatingState, createAppointment] = useApiCall((dto: AppointmentDTO) => 
        appointmentRepo.create(dto)
            .sideEffect(appointment => {
                console.log('Created', appointment);
                dataDispatch({ type: 'updateAppointment', appointment });
            })
    , []);

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in OwnAppointments.tsx, appointment state result match", state.error);
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    const appointments = dataState.appointments;
    const categories = dataState.categories;

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        {state instanceof Failed && <div>Error: {state.error.message}</div>}
        {state instanceof Idle && <div>Click to load.</div>}
        
        {state instanceof Loaded && 
        <>
            {creatingState instanceof Failed && <p>Error while creating appointment: {creatingState.error.message}</p>}
            {categories.length === 0
                ? <p>Can't create appointments without categories, create a category first!</p>
                : <AppointmentEditor onSubmit={createAppointment} categories={categories} />}
            <AppointmentAgenda appointments={appointments} />
        </>
        }
        </>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);

    if (loginState instanceof LoggedOut
        || (loginState instanceof LoggedIn && loginState.user.contractorPage === null)) 
        return <Redirect to='/' />;

    return (
        <DataProvider>
            <OwnAppointments user={loginState.user} />
        </DataProvider>
    )
}