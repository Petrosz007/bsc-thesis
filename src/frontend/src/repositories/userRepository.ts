import { IsLoggedInDTO } from "../logic/dtos";
import { User } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { safeApiFetchAs } from "./utilities";

export interface IUserRepository {
    getByUserName(userName: string): ResultPromise<User,Error>;
    getSelf(): ResultPromise<string,Error|'NotLoggedIn'>;
};

export class UserRepository implements IUserRepository {
    getByUserName = (userName: string): ResultPromise<User,Error> =>
        safeApiFetchAs<User>(`https://localhost:44347/User/Info/${userName}`, 'GET');

    getSelf = (): ResultPromise<string,Error|'NotLoggedIn'> =>
        safeApiFetchAs<IsLoggedInDTO>('https://localhost:44347/User/Self', 'GET')
            .andThen(res => res.userName === null 
                    ? ResultPromise.err('NotLoggedIn') 
                    : ResultPromise.ok(res.userName));
}
