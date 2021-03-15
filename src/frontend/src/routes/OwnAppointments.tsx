import AppointmentCard from "../components/AppointmentCard";
import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Appointment, User } from "../logic/entities";
import React, { useContext, useEffect } from "react";
import { Redirect } from "react-router";

const Appointments = ({ appointments }: { appointments: Appointment[] }) => {
    return (
        <>
        {appointments.map(appointment => 
            <AppointmentCard appointment={appointment} key={appointment.id} />
        )}
        </>
    );
}

const OwnAppointments = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);

    const [state, refreshData] = useApiCall(() =>
        appointmentRepo.getContractorsAppointments(user.userName)
            .sideEffect(appointments => {
                dataDispatch({ type: 'setAppointments', appointments });
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

    if (loginState instanceof LoggedOut
        || (loginState instanceof LoggedIn && loginState.user.contractorPage === null)) 
        return <Redirect to='/' />;

    return (
        <DataProvider>
            <OwnAppointments user={loginState.user} />
        </DataProvider>
    )
}