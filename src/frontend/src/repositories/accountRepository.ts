import { LoginDTO, RegisterDTO } from "../logic/dtos";
import { ResultPromise, Unit } from "../utilities/result";
import { safeApiFetchWithBodyAs, safeApiFetchWithBodyAsUnit } from "./utilities";

export interface IAccountRepository {
    login(userName: string, password: string): ResultPromise<Unit,Error>;
    logout(): ResultPromise<Unit,Error>;
    register(registerDto: RegisterDTO): ResultPromise<Unit,Error>;
}

export class AccountRepository implements IAccountRepository {
    login = (userName: string, password: string): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`https://localhost:44347/Account/Login`, 'POST', {
            userName, password
        } as LoginDTO);

    logout = (): ResultPromise<Unit,Error> => 
        safeApiFetchWithBodyAsUnit(`https://localhost:44347/Account/Logout`, 'POST');

    register = (registerDto: RegisterDTO): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`https://localhost:44347/Account/Register`, 'POST', registerDto);
}
