import React, { useEffect, useState } from "react";
import { useContext } from "react";
import { Redirect } from "react-router-dom";
import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Appointment, Category, User } from "../logic/entities";
import { downloadReportPdf } from "../logic/pdfReportGenerator";
import { createReport } from "../logic/reportGenerator";
import { Dictionary, groupBy, uniques } from "../utilities/listExtensions";

import './Report.scss';

const ReportDisplay = ({ owner, users, appointments, categories }: { owner: User, users: User[], appointments: Appointment[], categories: Category[] }) => {
    const [selectedUser, setSelectedUser] = useState(users[0]);

    const usersAppointments = appointments.filter(a => a.attendees.some(u => u.userName === selectedUser.userName));

    const report = createReport(usersAppointments, categories, owner, selectedUser);

    const totalPrice = report.entries.reduce((acc, x) => 
        acc + x.count * x.category.price
        , 0);

    return (
        <div>
            User:
            <select value={selectedUser.userName} onChange={e => setSelectedUser(users.find(u => u.userName === e.target.value) ?? users[0])}>
                {users.map(user =>
                    <option value={user.userName} key={user.userName}>{user.name} ({user.userName})</option>
                )}
            </select>
            {/* <AppointmentAgenda appointments={usersAppointments} /> */}
            <table className="report-table">
                <thead>
                    <tr>
                        <th>Kategória</th>
                        <th>Ár / alkalom</th>
                        <th>Alkalom</th>
                        <th>Összesen</th>
                    </tr>
                </thead>
                <tbody>
                    {report.entries.map(entry => 
                        <tr key={entry.category.id}>
                            <td>{entry.category.name}</td>
                            <td>{entry.category.price} Ft</td>
                            <td>{entry.count} db</td>
                            <td>{entry.count * entry.category.price} Ft</td>
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
            <button onClick={() => downloadReportPdf(report)}>Download Report</button>  
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
                : <ReportDisplay owner={user} users={users} appointments={appointments} categories={categories} />
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
