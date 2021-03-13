import { User } from "src/logic/entities";
import { ResultPromise } from "src/utilities/result";
import { safeApiFetch } from "./utilities";

export interface IUserRepository {
    getByUserName(userName: string): ResultPromise<User,Error>;
};

export class UserRepository implements IUserRepository {
    getByUserName = (userName: string): ResultPromise<User,Error> =>
        safeApiFetch(`https://localhost:44347/User/Info/${userName}`, 'GET')
        .mapAsync(async response => await response.json() as User);
}
