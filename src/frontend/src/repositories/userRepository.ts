import {IsLoggedInDTO, UserEditDTO} from "../logic/dtos";
import {User, UserSelfInfo} from "../logic/entities";
import {ResultPromise, Unit} from "../utilities/result";
import {safeApiFetchAs, safeApiFetchWithBodyAsUnit} from "./utilities";
import {Config} from "../config";

export interface IUserRepository {
    getByUserName(userName: string): ResultPromise<User,Error>;
    getSelf(): ResultPromise<string,Error|'NotLoggedIn'>;
    getContractors(): ResultPromise<User[], Error>;
    getSelfInfo(): ResultPromise<UserSelfInfo, Error>;
    update(dto: UserEditDTO): ResultPromise<Unit, Error>;
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

    getSelfInfo = (): ResultPromise<UserSelfInfo, Error> =>
        safeApiFetchAs<UserSelfInfo>(`${this.config.apiUrl}/User/SelfInfo`, 'GET');
    
    update = (dto: UserEditDTO): ResultPromise<Unit, Error> =>
        safeApiFetchWithBodyAsUnit(`${this.config.apiUrl}/User`, 'PUT', dto);
}
