import { useEffect, useState, StrictMode, useContext } from 'react';
import React from 'react';
import ReactDOM from 'react-dom';
import './App.scss';
import AppointmentCard from './components/AppointmentCard';
import LoginCard from './components/LoginCard';
import { Failed, Idle, Loaded, Loading, useApiCall } from './hooks/apiCallHooks';
import LoginProvider, { LoginContext } from './components/contexts/LoginProvider';
import NavBar from './components/NavBar';
import DataProvider, { DataContext } from './components/contexts/DataProvider';
import { DIContext } from './components/contexts/DIContext';

const App = () => {
    return (
        <DataProvider>
            <LoginProvider>
                <Main />
            </LoginProvider>
        </DataProvider>
    );
};

const Main = () => {
    const { loginState } = useContext(LoginContext);
    const { appointmentRepo } = useContext(DIContext);
    const { dataState, dataDispatch } = useContext(DataContext);

    const [appointmentId, setAppointmentId] = useState(1);

    const [state, refreshData] = useApiCall(() => 
        appointmentRepo.getById(appointmentId)
            .sideEffect(appointment => {
                dataDispatch({ type: 'updateAppointment', appointment });
            })
    , [appointmentId, loginState]);

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in index.tsx, appointment state result match", state.error);
        }
    }, [state]);

    const appointment = dataState.appointments.find(a => a.id === appointmentId);

    return (
        <>
            <NavBar />
            <LoginCard />
            <input type="number" value={appointmentId} onChange={e => setAppointmentId(parseInt(e.target.value))}/><br/>
            <button onClick={() => refreshData()}>Refresh that</button>
            {state instanceof Loading && <div>Loading...</div>}
            {state instanceof Failed && <div>Error: {state.error.message}</div>}
            {state instanceof Idle && <div>Click to load.</div>}

            {state instanceof Loaded && 
            (appointment === undefined 
                ? <div>Appointment not loaded yet.</div>
                :<AppointmentCard appointment={appointment}/>)}
        </>
    );
};

ReactDOM.render(
    <StrictMode>
        <App />
    </StrictMode>,
    document.getElementById('root')
);

 // Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
 // Learn more: https://www.snowpack.dev/concepts/hot-module-replacement
if (import.meta.hot) {
    import.meta.hot.accept();
}
