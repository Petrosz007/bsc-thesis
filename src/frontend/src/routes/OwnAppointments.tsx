import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { User } from "../logic/entities";
import React, { useContext, useEffect, useState } from "react";
import { Redirect } from "react-router";
import AppointmentAgenda from "../components/AppointmentAgenda";
import Modal from "../components/Modal";
import { AppointmentEditorCreate } from "../components/editors/AppointmentEditor";
import CategoryEditor from "../components/editors/CategoryEditor";
import { NotificationContext } from "../components/contexts/NotificationProvider";

import './OwnAppointments.scss'

const OwnAppointments = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo, categoryRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);
    const [isAppointmentModalOpen, setIsAppointmentModalOpen] = useState(false);
    const [isCategoryModalOpen, setIsCategoryModalOpen] = useState(false);

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

    useEffect(() => {
        if(state instanceof Failed) {
            console.error("Error in OwnAppointments.tsx, appointment state result match", state.error);
            notificationDispatch({ type: 'addError', message: `Error in OwnAppointments: ${state.error}` });
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
        
        {state instanceof Loaded && 
        <>
            {isCategoryModalOpen &&
                <Modal>
                    <CategoryEditor owner={user} onClose={() => setIsCategoryModalOpen(false)} />
                </Modal>
            }
            {isAppointmentModalOpen &&
                <Modal>
                    <AppointmentEditorCreate categories={categories} onClose={() => setIsAppointmentModalOpen(false)} />
                </Modal>
            }

            <button onClick={() => setIsCategoryModalOpen(true)}>Create Category</button>
            {categories.length === 0
                ? <p>Can't create appointments without categories, create a category first!</p>
                : <button onClick={() => setIsAppointmentModalOpen(true)}>Create Appointment</button>
            }

            <AppointmentAgenda appointments={appointments} categories={categories} />
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
