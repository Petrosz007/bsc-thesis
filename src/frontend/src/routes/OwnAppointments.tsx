import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { User } from "../logic/entities";
import React, { useContext, useEffect, useState } from "react";
import { Redirect } from "react-router";
import {AppointmentAgendaEditable} from "../components/AppointmentAgenda";
import Modal from "../components/Modal";
import { AppointmentEditorCreate } from "../components/editors/AppointmentEditor";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import {CategoriesEditable} from "../components/CategoryCard";

import './OwnAppointments.scss'
import {CategoryEditorCreate} from "../components/editors/CategoryEditor";
import {EditIcon, PlusIcon} from "../SVGs";

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
            notificationDispatch({ type: 'addError', message: `Hiba: ${state.error.message}` });
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
        <div className="ownAppointments">
            <Modal isOpen={isCategoryModalOpen}>
                <CategoryEditorCreate owner={user} onClose={() => setIsCategoryModalOpen(false)} />
            </Modal>
            <Modal isOpen={isAppointmentModalOpen}>
                <AppointmentEditorCreate categories={categories} onClose={() => setIsAppointmentModalOpen(false)} />
            </Modal>
            
            <div className="newButtons">
                <button onClick={() => setIsCategoryModalOpen(true)}><PlusIcon className="plusSVG" />Új kategória</button>
                {categories.length === 0 ||
                    <button onClick={() => setIsAppointmentModalOpen(true)}><PlusIcon className="plusSVG" />Új időpont</button>
                }
            </div>
            
            {categories.length === 0
                ? <p className="noCategoriesFound">Hozz létre egy kategóriát, hogy hirdethess időpontokat!</p>
                : <>
                <CategoriesEditable owner={user} categories={categories} />
                <AppointmentAgendaEditable appointments={appointments} categories={categories} />
                </>
            }
        </div>
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
