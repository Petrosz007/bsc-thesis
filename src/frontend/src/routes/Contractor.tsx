import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import React, { useContext, useEffect } from "react";
import { Switch, useParams, useRouteMatch } from "react-router";
import { Route } from "react-router-dom";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import {AppointmentAgenda} from "../components/AppointmentAgenda";
import {ContractorCard} from "../components/ContractorCard";

import './Contractor.scss';

const ContractorPage = () => {
    const { contractorUserName } = useParams<{ contractorUserName: string }>();
    const { loginState } = useContext(LoginContext);
    const { appointmentRepo, userRepo } = useContext(DIContext);
    const { dataState, dataDispatch } = useContext(DataContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [state, refreshData] = useApiCall(() => {
        const appointmentsResult = appointmentRepo.getContractorsAppointments(contractorUserName);
        const contractorResult = userRepo.getByUserName(contractorUserName);

        return appointmentsResult
            .andThen(appointments =>
                contractorResult.map(user => [appointments, user] as const)
            )
            .sideEffect(([appointments, contractor]) => {
                dataDispatch({ type: 'setAppointments', appointments });
                dataDispatch({ type: 'updateUser', user: contractor });
            });
        }, [contractorUserName, loginState]);
    
    useEffect(() => {
        if(state instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `${state.error.message}` });
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        
        {state instanceof Loaded && 
        <div className="contractorLayout">
            <ContractorCard contractor={state.value[1]}/>
            <AppointmentAgenda appointments={dataState.appointments} showFull={false} />
        </div>
        }
        </>
    );
}

const ContractorBrowser = () => {
    const { userRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const [state, refreshData] = useApiCall(() =>
        userRepo.getContractors()
    , []);

    useEffect(() => {
        if(state instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `${state.error.message}` });
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);
    
    return (
        <>
            {state instanceof Loading && <div>Loading...</div>}

            {state instanceof Loaded &&
            <div className="contractorBrowser">
                {state.value.length === 0
                    ? <p className="noContractorsToShow">Még egy vállalkozó sem regisztrált a weboldalra.</p>
                    : state.value.map(contractor =>
                        <ContractorCard contractor={contractor} key={contractor.userName}/>
                )}
            </div>
            }
        </>
    );
}

export default () => {
    const match = useRouteMatch();

    return (
        <DataProvider>
            <div>

                <Switch>
                    <Route path={`${match.path}/:contractorUserName`}>
                        <ContractorPage />
                    </Route>
                    <Route path={match.path}>
                        <ContractorBrowser />
                    </Route>
                </Switch>
            </div>
        </DataProvider>
    );
}
