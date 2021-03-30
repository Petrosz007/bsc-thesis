import React, { useContext, useEffect, useState } from "react";
import { Redirect, useHistory } from "react-router-dom";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoginContext } from "../components/contexts/LoginProvider";
import { Failed, Loaded, Loading, useApiCall, useLogin } from "../hooks/apiCallHooks";
import { useEffectAsync } from "../hooks/utilities";
import { RegisterDTO } from "../logic/dtos";
import { ContractorPage } from "../logic/entities";

const ContractorPageInputs = ({ state, setState }: { state: ContractorPage, setState: React.Dispatch<React.SetStateAction<ContractorPage>> }) => {
    const handleContractorChange = (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setState(prevState => ({
            ...prevState,
            [event.target.name]: event.target.value,
        }));
    };

    return (
    <>
        <label htmlFor="title">Title:</label>
        <input type="text" name="title" value={state.title} onChange={handleContractorChange}/><br/>

        <label htmlFor="bio">Bio:</label>
        <input type="text" name="bio" value={state.bio} onChange={handleContractorChange}/><br/>
    </>
    );
}

const RegisterForm = ({ onSubmit }: { onSubmit: (_x: RegisterDTO) => void }) => {
    const [isContractor, setIsContractor] = useState(false);
    const [contractorPageState, setContractorPageState] = useState<ContractorPage>({
        title: '',
        bio: '',
    });

    const [state, setState] = useState<RegisterDTO>({
        userName: '',
        email: '',
        name: '',
        password: '',
        passwordConfirmation: '',
        contractorPage: null,
    });

    const handleChange = (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setState(prevState => ({
            ...prevState,
            [event.target.name]: event.target.value,
        }));
    };

    const handleSubmit = (event: React.ChangeEvent<HTMLFormElement>) => {
        onSubmit({
            ...state,
            contractorPage: isContractor ? contractorPageState : null,
        });
        event.preventDefault();
    }

    return (
        <form onSubmit={handleSubmit}>
            {/* <input type="date" value={state.startTime}/> */}
            <label htmlFor="userName">Username:</label>
            <input type="text" name="userName" value={state.userName} onChange={handleChange} /><br/>

            <label htmlFor="email">Email:</label>
            <input type="email" name="email" value={state.email} onChange={handleChange} /><br/>

            <label htmlFor="name">Name:</label>
            <input type="text" name="name" value={state.name} onChange={handleChange} /><br/>

            <label htmlFor="password">Password:</label>
            <input type="password" name="password" value={state.password} onChange={handleChange} /><br/>

            <label htmlFor="passwordConfirmation">Password confirmation:</label>
            <input type="password" name="passwordConfirmation" value={state.passwordConfirmation} onChange={handleChange} /><br/>

            <label htmlFor="isContractor">Register as a contractor? </label>
            <input type="checkbox" name="isContractor" checked={isContractor} onChange={e => setIsContractor(prevState => !prevState)} /><br/>

            {isContractor &&
                <ContractorPageInputs state={contractorPageState} setState={setContractorPageState} />
            }

            <input type="submit" value="Submit"/>
        </form>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);
    const { accountRepo } = useContext(DIContext);
    const history = useHistory();

    const [_loginStatus, login] = useLogin();


    const [registerStatus, register] = useApiCall((dto: RegisterDTO) => 
        accountRepo.register(dto)
            .map(_ => dto)
    , []);

    useEffectAsync(async () => {
        if(registerStatus instanceof Loaded) {
            await login(registerStatus.value.userName, registerStatus.value.password);
            history.push('/');
        }
    }, [registerStatus]);
    
    // Before the login hook finishes, the global login state changes => rerenders this component =>
    // redirects it to another page => Unmounts these components => early return for hooks
    // This can be avoided by checking if the user registered just now 
    if (loginState instanceof LoggedIn && !(registerStatus instanceof Loaded))
        return <Redirect to='/' />;

    return (
        <>
            {registerStatus instanceof Failed && <div>Error: {registerStatus.error.message}</div> }
            {registerStatus instanceof Loading && <div>Registering...</div> }
            <RegisterForm onSubmit={register}/>
        </>
    );
}