import React, {useEffect, useMemo, useState} from "react";
import { useContext } from "react";
import { Redirect } from "react-router-dom";
import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import {Appointment, Category, Report, User} from "../logic/entities";
import { downloadReportPdf } from "../logic/pdfReportGenerator";
import { createReport } from "../logic/reportGenerator";
import { Dictionary, groupBy, uniques } from "../utilities/listExtensions";
import Select from "react-select";

import './Report.scss';
import UserName from "../components/UserName";
import {DateTime, Interval} from "luxon";
import {DatePicker, DateRangePicker} from "../components/inputs/DatePicker";
import UserSelector from "../components/inputs/UserSelector";

const ReportTable = ({ report }: { report: Report }) => {
    const totalPrice = report.entries.reduce((acc, x) =>
        acc + x.count * x.category.price
        , 0);
    
    return (
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
    );
}

const ReportDisplay = ({ owner, users, appointments, categories }: { owner: User, users: User[], appointments: Appointment[], categories: Category[] }) => {
    const startOfTheMonth = useMemo(() => DateTime.now().set({ day: 1, hour: 0, minute: 0, second: 0, millisecond: 0 }), []);
    const endOfTheMonth = useMemo(() => DateTime.now().set({ day: DateTime.now().daysInMonth, hour: 23, minute: 59, second: 59, millisecond: 59 }), []);
    
    const [selectedUser, setSelectedUser] = useState(users[0]);
    const [dateInterval, setDateInterval] = useState(Interval.fromDateTimes(startOfTheMonth, endOfTheMonth));
    
    const usersAppointments = useMemo(() => {
        const filtered =  appointments
            .filter(a => a.attendees.some(u => u.userName === selectedUser.userName))
            .filter(a => dateInterval.contains(a.startTime))
        const sorted = [...filtered].sort((left, right) => left.startTime.toMillis() - right.startTime.toMillis());
        return sorted;
    }, [appointments, selectedUser, dateInterval]);

    const report = useMemo(() => createReport(usersAppointments, categories, dateInterval, owner, selectedUser), 
        [usersAppointments, categories, owner, selectedUser]);

    return (
        <div>
            User:
            <UserSelector selectedUser={selectedUser} setSelectedUser={setSelectedUser} users={users} />

            <DateRangePicker value={dateInterval} onChange={setDateInterval} />
            <ReportTable report={report} />
            <ul>
            {usersAppointments.map(a => 
                <li key={a.id}>{a.startTime.toLocaleString(DateTime.DATE_MED_WITH_WEEKDAY)} - {a.category.name}</li>
            )}
            </ul>
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
