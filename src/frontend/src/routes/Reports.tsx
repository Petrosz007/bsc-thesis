import React, { useEffect, useState } from "react";
import { useContext } from "react";
import { Redirect } from "react-router-dom";
import AppointmentAgenda from "../components/AppointmentAgenda";
import AppointmentCard from "../components/AppointmentCard";
import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Appointment, Category, User } from "../logic/entities";
import { Dictionary, groupBy, uniques } from "../utilities/listExtensions";

import './Report.scss';

const ReportDisplay = ({ users, appointments, categories }: { users: User[], appointments: Appointment[], categories: Category[] }) => {
    useEffect(() => console.log(categories), []);
    const [selectedUser, setSelectedUser] = useState(users[0]);

    const usersAppointments = appointments.filter(a => a.attendees.some(u => u.userName === selectedUser.userName));

    const categoryGroups = groupBy(usersAppointments, a => `${a.category.id}`);
    const usersCategories = Dictionary.keys(categoryGroups)
        .map(id => parseInt(id))
        .map(id => categories.find(c => c.id === id) ?? categories[0]);

    const totalPrice = usersCategories.reduce((acc, x) => 
        acc + categoryGroups[x.id].length * x.price
        , 0);

    return (
        <div>
            User:
            <select value={selectedUser.userName} onChange={e => setSelectedUser(users.find(u => u.userName === e.target.value) ?? users[0])}>
                {users.map(user =>
                    <option value={user.userName}>{user.name} ({user.userName})</option>
                )}
            </select>
            {/* <AppointmentAgenda appointments={usersAppointments} /> */}
            <table className="report-table">
                <thead>
                    <th>Kategória</th>
                    <th>Ár / alkalom</th>
                    <th>Alkalom</th>
                    <th>Összesen</th>
                </thead>
                <tbody>
                    {usersCategories.map(category => 
                        <tr key={category.id}>
                            <td>{category.name}</td>
                            <td>{category.price} Ft</td>
                            <td>{categoryGroups[category.id].length} db</td>
                            <td>{categoryGroups[category.id].length * category.price} Ft</td>    
                        </tr>
                    )}
                    <tr>
                        <td></td>
                        <td></td>
                        <td>Összesen:</td>
                        <td>{totalPrice} Ft</td>
                    </tr>
                </tbody>
            </table>    
        </div>
    );
}

const Reports = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [state, refreshData] = useApiCall(() =>
        appointmentRepo.getContractorsAppointments(user.userName)
            .sideEffect(appointments => {
                const attendees = uniques(appointments.flatMap(a => a.attendees), u => u.userName);
                const categories = uniques(appointments.map(a => a.category), c => `${c.id}`);

                dataDispatch({ type: 'setAppointments', appointments });
                dataDispatch({ type: 'setUsers', users: attendees });
                dataDispatch({ type: 'setCategories', categories });
            })
    , []);

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in Report.tsx, appointment state result match", state.error);
            notificationDispatch({ type: 'addError', message: `Error in Report: ${state.error}` });
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    const appointments = dataState.appointments;
    const users = dataState.users;
    const categories = dataState.categories;

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        
        {state instanceof Loaded && 
            (users.length === 0
                ? <div>No users attended your appointments.</div>
                : <ReportDisplay users={users} appointments={appointments} categories={categories} />
            )}
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
            <Reports user={loginState.user} />
        </DataProvider>
    )
}
