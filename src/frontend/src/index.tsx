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

const App = () => {
    const userRepo = new UserRepository();
    const categoryRepo = new CategoryRepository(userRepo);
    const appointmentRepo = new AppointmentRepository(userRepo, categoryRepo);

    const [appointment, setAppointment] = useState<Appointment>();
    
    useEffect(() => {
        refreshData()
    }, []);

    const refreshData = useCallback(() => {
        setAppointment(undefined);
        appointmentRepo.getById(3)
            .then(c => setAppointment(c))
            .catch(err => console.error(err));
    }, [categoryRepo]);

    return (
        <>
            {/* <h1>my first snowpack+react app</h1>
            <h2>hello ❄️Snowpack❄️</h2>
            <p>Is this really that fast? Holy moly</p>
            <h1>Time is ticking awaaay {count} awaay</h1>
            <div id="target0">
                <p>Lorem ipsum dolor sit, amet consectetur adipisicing elit. Similique a quo maiores dolore sequi. Vitae nam facere labore, expedita totam, eligendi dolor eum veniam accusamus assumenda enim eos. Impedit, sint.</p>
            </div> */}
            {appointment === undefined
                ? <p>Loading...</p>
                : <AppointmentCard appointment={appointment}/>
            }
            <button onClick={() => refreshData()}>Refresh that</button>
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
