import { IsLoggedInDTO } from "../logic/dtos";
import { User } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { safeApiFetchAs } from "./utilities";
import {Config} from "../config";

export interface IUserRepository {
    getByUserName(userName: string): ResultPromise<User,Error>;
    getSelf(): ResultPromise<string,Error|'NotLoggedIn'>;
    getContractors(): ResultPromise<User[], Error>;
};

export class UserRepository implements IUserRepository {
    constructor(
        private readonly config: Config
    ) {}
    
    getByUserName = (userName: string): ResultPromise<User,Error> =>
        safeApiFetchAs<User>(`${this.config.apiUrl}/User/Info/${userName}`, 'GET');

    getSelf = (): ResultPromise<string,Error|'NotLoggedIn'> =>
        safeApiFetchAs<IsLoggedInDTO>(`${this.config.apiUrl}/User/Self`, 'GET')
            .andThen(res => res.userName === null 
                    ? ResultPromise.err('NotLoggedIn') 
                    : ResultPromise.ok(res.userName));

    getContractors = (): ResultPromise<User[], Error> =>
        safeApiFetchAs<User[]>(`${this.config.apiUrl}/User/Contractors`, 'GET');
}
