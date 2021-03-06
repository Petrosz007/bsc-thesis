import { User } from "src/logic/entities";

export interface IUserRepository {
    getByUserName(userName: string) : Promise<User>;
};

export class UserRepository implements IUserRepository {
    async getByUserName(userName: string) : Promise<User> {
        const response = await fetch(`https://localhost:44347/User/Info/${userName}`);
        if(!response.ok) {
            throw new Error(`${response.status}: ${await response.text()}`)
        }
        
        const user = await response.json() as User;
        return user;
    }
}
