import { useEffect, useState, StrictMode, useCallback } from 'react';
import React from 'react';
import ReactDOM from 'react-dom';
import './App.scss';
import { Appointment, Category, User } from './logic/entities';
import { UserRepository } from './repositories/userRepository';
import CategoryCard from './components/CategoryCard';
import { CategoryRepository } from './repositories/categoryRepository';
import { AppointmentRepository } from './repositories/appointmentRepository';
import AppointmentCard from './components/AppointmentCard';
import LoginCard from './components/LoginCard';
import { Failed, Idle, Loaded, Loading, useApiCall } from './hooks/apiCallHooks';
import LoginProvider from './components/contexts/LoginProvider';

const App = () => {
    const userRepo = new UserRepository();
    const categoryRepo = new CategoryRepository(userRepo);
    const appointmentRepo = new AppointmentRepository(userRepo, categoryRepo);

    const [appointmentId, setAppointmentId] = useState(1);

    const [state, refreshData] = useApiCall(() => appointmentRepo.getById(appointmentId), [appointmentId]);

    return (
        <LoginProvider>
            <LoginCard />
            <input type="number" value={appointmentId} onChange={e => setAppointmentId(parseInt(e.target.value))}/><br/>
            <button onClick={() => refreshData()}>Refresh that</button>
            {state instanceof Loading && <div>Loading...</div>}
            {state instanceof Failed && <div>Error: {state.error.message}</div>}
            {state instanceof Idle && <div>Click to load.</div>}
            {state instanceof Loaded && <AppointmentCard appointment={state.value}/>}
        </LoginProvider>
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
