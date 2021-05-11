import React, { useContext, useEffect, useState } from "react";
import { Redirect, useHistory } from "react-router-dom";
import { DIContext } from "../components/contexts/DIContext";
import { LoggedIn, LoginContext } from "../components/contexts/LoginProvider";
import { NotificationContext } from "../components/contexts/NotificationProvider";
import { Failed, Loaded, Loading, useApiCall, useLogin } from "../hooks/apiCallHooks";
import { useEffectAsync } from "../hooks/utilities";
import { RegisterDTO } from "../logic/dtos";
import { ContractorPage } from "../logic/entities";

import './Register.scss';

const ContractorPageInputs = ({ state, setState }: { state: ContractorPage, setState: React.Dispatch<React.SetStateAction<ContractorPage>> }) => {
    const handleContractorChange = (event: React.ChangeEvent<HTMLInputElement & HTMLSelectElement & HTMLTextAreaElement>) => {
        setState(prevState => ({
            ...prevState,
            [event.target.name]: event.target.value,
        }));
    };

    return (
    <>
        <div>
            <label htmlFor="title">Foglalkozás</label>
            <input type="text" name="title" value={state.title} required placeholder="Angol tanár, Személyi edző..." onChange={handleContractorChange}/>
        </div>

        <div>
            <label htmlFor="bio">Magamról</label>
            <textarea name="bio" value={state.bio} required placeholder="Rövid profil leírás, amit az ügyfelek elolvashatnak..." onChange={handleContractorChange} rows={3}/>
        </div>
    </>
    );
}

const RegisterForm = ({ onSubmit }: { onSubmit: (_x: RegisterDTO) => void }) => {
    const { notificationDispatch } = useContext(NotificationContext);
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
        event.preventDefault();
        
        if(state.password !== state.passwordConfirmation) {
            notificationDispatch({ type: 'addError', message: 'Nem egyezik a jelszó és jelszó ismétlés!' });
            return;
        }
        onSubmit({
            ...state,
            contractorPage: isContractor ? contractorPageState : null,
        });
    }

    return (
        <div className="registerPage">
            <h2>Regisztráció</h2>
            <form className="registerForm" onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="userName">Felhasználónév</label>
                    <input type="text" name="userName" value={state.userName} required
                           placeholder="felhasznalo42" 
                           onChange={handleChange}
                           pattern="[a-zA-Z0-9_]{3,25}"
                           title="3-25 karakter, a-z kisebetű, A-Z nagybetű, 0-9 szám"
                    />
                </div>
    
                <div>
                    <label htmlFor="email">Email</label>
                    <input type="email" name="email" value={state.email} required placeholder="felhasznalo@example.com" onChange={handleChange} />
                </div>
    
                <div>
                    <label htmlFor="name">Név</label>
                    <input type="text" name="name" value={state.name} required placeholder="Felhasználó Ferenc" onChange={handleChange} />
                </div>
    
                <div>
                    <label htmlFor="password">Jelszó</label>
                    <input type="password" name="password" value={state.password} required 
                           placeholder=" " autoComplete="new-password" onChange={handleChange} 
                           minLength={6}
                    />
                </div>
    
                <div>
                    <label htmlFor="passwordConfirmation">Jelszó megerősítés</label>
                    <input type="password" name="passwordConfirmation" value={state.passwordConfirmation} required 
                           placeholder=" " onChange={handleChange}
                           minLength={6}
                    />
                </div>
    
                <div>
                    <label htmlFor="isContractor">Regisztráció vállalkozóként?
                        <input type="checkbox" name="isContractor" checked={isContractor} onChange={e => setIsContractor(prevState => !prevState)} />
                    </label>
                </div>
    
                {isContractor &&
                    <ContractorPageInputs state={contractorPageState} setState={setContractorPageState} />
                }
    
                <input type="submit" value="Regisztráció"/>
            </form>
        </div>
    );
}

export default () => {
    const { loginState } = useContext(LoginContext);
    const { accountRepo } = useContext(DIContext);
    const { notificationDispatch } = useContext(NotificationContext);

    const history = useHistory();

    const [_loginStatus, login] = useLogin();


    const [registerStatus, register] = useApiCall((dto: RegisterDTO) => 
        accountRepo.register(dto)
            .map(_ => dto)
    , []);

    useEffectAsync(async () => {
        if(registerStatus instanceof Failed) {
            notificationDispatch({ type: 'addError', message: `HIba regisztráció közben: ${registerStatus.error}` });
        }
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
            {registerStatus instanceof Loading && <div>Registering...</div> }
            <RegisterForm onSubmit={register}/>
        </>
    );
}