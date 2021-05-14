import { LoginDTO, RegisterDTO } from "../logic/dtos";
import { ResultPromise, Unit } from "../utilities/result";
import { safeApiFetchWithBodyAsUnit } from "./utilities";
import {Config} from "../config";

export interface IAccountRepository {
    login(userName: string, password: string): ResultPromise<Unit,Error>;
    logout(): ResultPromise<Unit,Error>;
    register(registerDto: RegisterDTO): ResultPromise<Unit,Error>;
}

export class AccountRepository implements IAccountRepository {
    constructor(
        private readonly config: Config
    ) {}
    
    login = (userName: string, password: string): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/Account/Login`, 'POST', {
            userName, password
        } as LoginDTO);

    logout = (): ResultPromise<Unit,Error> => 
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/Account/Logout`, 'POST');

    register = (registerDto: RegisterDTO): ResultPromise<Unit,Error> =>
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/Account/Register`, 'POST', registerDto);
}
