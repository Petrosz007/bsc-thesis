import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import React, { useContext, useEffect } from "react";
import { Switch, useParams, useRouteMatch } from "react-router";
import { Route } from "react-router-dom";
import { User } from "../logic/entities";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import {AppointmentAgenda} from "../components/AppointmentAgenda";

const ContractorInfo = ({ user }: { user: User }) => {
    return (
        <div>
            <h2>{user.name}</h2>
            <p>{user.contractorPage?.title}</p>
            <p>{user.contractorPage?.bio}</p>
        </div>
    );
}

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
            console.error("Error in index.tsx, appointment state result match", state.error);
            notificationDispatch({ type: 'addError', message: `Error in Contractor: ${state.error}` });
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        
        {state instanceof Loaded && 
        <>
            <ContractorInfo user={state.value[1]}/>
            <AppointmentAgenda appointments={dataState.appointments} />
        </>
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
                        <h3>No contractor username in url</h3>
                    </Route>
                </Switch>
            </div>
        </DataProvider>
    );
}
