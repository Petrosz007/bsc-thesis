import AppointmentCard from "../components/AppointmentCard";
import DataProvider, { DataContext } from "../components/contexts/DataProvider";
import { DIContext } from "../components/contexts/DIContext";
import { LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import React, { useContext, useEffect } from "react";
import { Switch, useParams, useRouteMatch } from "react-router";
import { Route } from "react-router-dom";
import { Appointment, User } from "../logic/entities";
import { Dictionary, groupBy } from "../utilities/listExtensions";
import AppointmentAgenda from "../components/AppointmentAgenda";

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
        }
        else if(state instanceof Idle) {
            refreshData();
        }
    }, [state]);

    return (
        <>
        {state instanceof Loading && <div>Loading...</div>}
        {state instanceof Failed && <div>Error: {state.error.message}</div>}
        {state instanceof Idle && <div>Click to load.</div>}
        
        {state instanceof Loaded && 
        <>
            <ContractorInfo user={state.value[1]}/>
            <AppointmentAgenda appointments={dataState.appointments}/>
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
