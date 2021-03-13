import { LoginDTO } from "logic/dtos";
import { ResultPromise } from "src/utilities/result";
import { safeApiFetchWithBody } from "./utilities";

export interface IAccountRepository {
    login(userName: string, password: string): ResultPromise<void,Error>;
    logout(): ResultPromise<void,Error>;
}

export class AccountRepository implements IAccountRepository {
    login = (userName: string, password: string): ResultPromise<void,Error> => {
        const loginDto: LoginDTO = { userName, password };
        return safeApiFetchWithBody(`https://localhost:44347/Account/Login`, 'POST', loginDto)
            .map(_result => {});
    }

    logout = (): ResultPromise<void,Error> => 
        safeApiFetchWithBody(`https://localhost:44347/Account/Logout`, 'POST')
            .map(_result => {});
}
