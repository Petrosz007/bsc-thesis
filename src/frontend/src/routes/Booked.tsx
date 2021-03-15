import AppointmentCard from "../components/AppointmentCard";
import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Appointment, User } from "../logic/entities";
import React, { useContext, useEffect } from "react";
import { Redirect } from "react-router";

const Appointments = ({ appointments }: { appointments: Appointment[] }) => {
    if(appointments.length === 0) return <p>No booked appointments</p>
    return (
        <>
        {appointments.map(appointment => 
            <AppointmentCard appointment={appointment} key={appointment.id} />
        )}
        </>
    );
}

const BookedAppointments = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);

    const [state, refreshData] = useApiCall(() =>
        appointmentRepo.getBooked()
            .sideEffect(appointments => {
                dataDispatch({ type: 'setAppointments', appointments });
            })
    , []);

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in Booked.tsx, appointment state result match", state.error);
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    const appointments = dataState.appointments.filter(a => a.attendees.some(u => u.userName === user.userName));

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        {state instanceof Failed && <div>Error: {state.error.message}</div>}
        {state instanceof Idle && <div>Click to load.</div>}
        
        {state instanceof Loaded && 
            <Appointments appointments={appointments} />
        }
        </>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);

    return (
        <DataProvider>
            {loginState instanceof LoggedOut &&
                <Redirect to='/' />
            }
            {loginState instanceof LoggedIn &&
                <BookedAppointments user={loginState.user} />
            }
        </DataProvider>
    )
}