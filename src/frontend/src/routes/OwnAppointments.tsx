import DataProvider, { DataContext } from "../components/contexts/DataProvider"
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoggedOut, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Idle, Loaded, Loading, useApiCall } from "../hooks/apiCallHooks";
import { Category, User } from "../logic/entities";
import React, { useCallback, useContext, useEffect, useState } from "react";
import { Redirect } from "react-router";
import { AppointmentDTO, CategoryDTO } from "../logic/dtos";
import AppointmentAgenda from "../components/AppointmentAgenda";
import UserAdder from "../components/UserAdder";

import './OwnAppointments.scss'
import Modal from "../components/Modal";
interface AppointmentEditdata {
    startTimeDate: string;
    startTimeTime: string;
    endTimeDate: string;
    endTimeTime: string;
    categoryId: number;
    maxAttendees: number;

    createAnother: boolean;
}

interface CategoryEditdata {
    name: string;
    description: string;
    everyoneAllowed: boolean;
    maxAttendees: number;
    price: number;

    createAnother: boolean;
}

const AppointmentEditor = ({ categories, onClose }: {
    categories: Category[],
    onClose: () => void,
}) => {
    const { dataDispatch } = useContext(DataContext);
    const { appointmentRepo } = useContext(DIContext);
    const initialState: AppointmentEditdata = {
        startTimeDate: new Date().toISOString().slice(0,10),
        startTimeTime: new Date().toISOString().slice(11,16),
        endTimeDate: new Date(Date.now() + 60*60*1000).toISOString().slice(0,10),
        endTimeTime: new Date(Date.now() + 60*60*1000).toISOString().slice(11,16),
        categoryId: categories[0].id,
        maxAttendees: 1,
        createAnother: false,
    };

    const [users, setUsers] = useState<User[]>([]);
    const [closeAfterLoad, setCloseAfterLoad] = useState(false);
    const [state, setState] = useState(initialState);

    const [createAppointmentState, createAppointment] = useApiCall((dto: AppointmentDTO) => 
        appointmentRepo.create(dto)
            .sideEffect(appointment => {
                console.log('Created', appointment);
                dataDispatch({ type: 'updateAppointment', appointment });
            })
    , []);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement & HTMLSelectElement>) => {
        const value = event.target.type === 'checkbox' 
            ? event.target.checked
            : event.target.value;

        setState({
            ...state,
            [event.target.name]: value,
        });
    };

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        setCloseAfterLoad(!state.createAnother);
        createAppointment({ 
            ...state,
            startTime: new Date(`${state.startTimeDate} ${state.startTimeTime}`).toISOString(),
            endTime: new Date(`${state.endTimeDate} ${state.endTimeTime}`).toISOString(),
            attendeeUserNames: users.map(user => user.userName),
            id: 0,
        }, !state.createAnother);

        event.preventDefault();
    }

    useEffect(() => {
        if(createAppointmentState instanceof Failed) {
            console.error('Error in AppointmentEditor: ', createAppointmentState.error);
        }
        if(createAppointmentState instanceof Loaded){
            if(closeAfterLoad) {
                onClose();
            } else {
                setState(prevState => ({
                    ...initialState,
                    createAnother: prevState.createAnother,
                }));
                setUsers([]);
            }
        }
    }, [createAppointmentState, closeAfterLoad]);

    return (
        <>
        {createAppointmentState instanceof Failed && <p>Error while creating appointment: {createAppointmentState.error.message}</p>}
        <form onSubmit={handleSubmit}>
            Start:
            <input type="date" name="startTimeDate" value={state.startTimeDate} onChange={handleChange} />
            <input type="time" name="startTimeTime" value={state.startTimeTime} onChange={handleChange} />
            <br/>
            End:
            <input type="date" name="endTimeDate" value={state.endTimeDate} onChange={handleChange} />
            <input type="time" name="endTimeTime" value={state.endTimeTime} onChange={handleChange} />
            <br/>
            Category:
            <select name="categoryId" value={state.categoryId} onChange={handleChange}>
                {categories.map(category => 
                    <option value={category.id} key={category.id}>{category.name}</option>
                )}
            </select><br/>
            MaxAttendees: <input type="number" name="maxAttendees" value={state.maxAttendees} onChange={handleChange} /><br/>
            <UserAdder users={users} setUsers={setUsers} />
            Create another: <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
            <input type="submit" value="Submit"/>
            <button onClick={e => {e.preventDefault(); onClose();}}>Cancel</button>
        </form>
        </>
    );
}

const CategoryEditor = ({ owner, onClose }: {
    owner: User,
    onClose: () => void,
}) => {
    const { dataDispatch } = useContext(DataContext);
    const { categoryRepo } = useContext(DIContext);

    const [closeAfterLoad, setCloseAfterLoad] = useState(false);
    const [users, setUsers] = useState<User[]>([]);

    const [createCategoryState, createCategory] = useApiCall((dto: CategoryDTO) => 
        categoryRepo.create(dto)
            .sideEffect(category => {
                console.log('Created', category);
                dataDispatch({ type: 'updateCategory', category });
            })
    , []);

    const initialState: CategoryEditdata = {
        name: '',
        description: '',
        everyoneAllowed: true,
        maxAttendees: 1,
        price: 0,
        createAnother: true,
    };

    const [state, setState] = useState(initialState);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement & HTMLSelectElement>) => {
        const value = event.target.type === 'checkbox' 
            ? event.target.checked
            : event.target.value;

        setState({
            ...state,
            [event.target.name]: value,
        });
    };

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        setCloseAfterLoad(!state.createAnother);
        createCategory({ 
            ...state,
            allowedUserNames: users.map(u => u.userName),
            ownerUserName: owner.userName,
            id: 0,
        })
        event.preventDefault();
    }

    useEffect(() => {
        if(createCategoryState instanceof Failed) {
            console.error('Error in CategoryEditor: ', createCategoryState.error);
        }
        if(createCategoryState instanceof Loaded){
            if(closeAfterLoad) {
                onClose();
            } else {
                setState(prevState => ({
                    ...initialState,
                    createAnother: prevState.createAnother,
                }));
                setUsers([]);
            }
        }
    }, [createCategoryState, closeAfterLoad]);

    return (
        <>
        {createCategoryState instanceof Failed && <p>Error while creating category: {createCategoryState.error.message}</p>}
        <form onSubmit={handleSubmit}>
            Name: <input type="text" name="name" value={state.name} onChange={handleChange} /><br/>
            Description: <input type="text" name="description" value={state.description} onChange={handleChange} /><br/>
            Everyone allowed: <input type="checkbox" name="everyoneAllowed" checked={state.everyoneAllowed} onChange={handleChange} /><br/>
            MaxAttendees: <input type="number" name="maxAttendees" value={state.maxAttendees} onChange={handleChange} /><br/>
            Price: <input type="number" name="price" value={state.price} onChange={handleChange} /><br/>
            <UserAdder users={users} setUsers={setUsers} />
            Create another: <input type="checkbox" name="createAnother" checked={state.createAnother} onChange={handleChange} />
            <input type="submit" value="Submit"/>
            <button onClick={e => {e.preventDefault(); onClose();}}>Cancel</button>
        </form>
        </>
    );
}

const OwnAppointments = ({ user }: { user: User }) => {
    const { dataState, dataDispatch } = useContext(DataContext);
    const { appointmentRepo, categoryRepo } = useContext(DIContext);
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
        {state instanceof Failed && <div>Error: {state.error.message}</div>}
        {state instanceof Idle && <div>Click to load.</div>}
        
        {state instanceof Loaded && 
        <>
            {isCategoryModalOpen &&
                <Modal>
                    <CategoryEditor owner={user} onClose={() => setIsCategoryModalOpen(false)} />
                </Modal>
            }
            {isAppointmentModalOpen &&
                <Modal>
                    <AppointmentEditor categories={categories} onClose={() => setIsAppointmentModalOpen(false)} />
                </Modal>
            }

            <button onClick={() => setIsCategoryModalOpen(true)}>Create Category</button>
            {categories.length === 0
                ? <p>Can't create appointments without categories, create a category first!</p>
                : <button onClick={() => setIsAppointmentModalOpen(true)}>Create Appointment</button>
            }

            <AppointmentAgenda appointments={appointments} />
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
