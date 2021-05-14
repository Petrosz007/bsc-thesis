// import React, { useEffect } from "react";
// import { useCallback, useContext, useState } from "react";
// import { Redirect } from "react-router";
// import { Failed, Idle, Loaded, Loading, useApiCall, useLogin, useLogout } from "../hooks/apiCallHooks";
// import { LoggedIn, LoggedOut, LoginContext } from "./contexts/LoginProvider";
// import { NotificationContext } from "./contexts/NotificationProvider";

// export default () => {
//     const { loginState } = useContext(LoginContext);
//     const { notificationDispatch } = useContext(NotificationContext);
    
//     const [userName, setUserName] = useState('customer1');
//     const [password, setPassword] = useState('kebab');

//     const [loginStatus, login] = useLogin();

//     useEffect(() => {
//         if(loginStatus instanceof Failed) {
//             notificationDispatch({ type: 'addError', message: `Hiba bejelentkezésnél: ${loginStatus.error.message}` });
//         }
//     }, [loginStatus]);

//     if(loginState instanceof LoggedIn && loginStatus instanceof Loaded)
//         return <Redirect to="/" />;

//     if(loginStatus instanceof Loading) return <div>Bejelentkezés...</div>

//     return (
//         <form onSubmit={() => login(userName, password)}>
//             Felhasználónév <input type="text" value={userName} required={true} onChange={e => setUserName(e.target.value)}/><br/>
//             Jelszó <input type="password" value={password} required={true} autoComplete="current-password" onChange={e => setPassword(e.target.value)}/><br/>
//             <input type="submit" value="Bejelentkezés" />
//         </form>
//     );
// };