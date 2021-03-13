import { LoginDTO } from "../logic/dtos";
import { apiFetchWithBody } from "./utilities";

export interface IAccountRepository {
    login(userName: string, password: string): Promise<void>;
    logout(): Promise<void>;
}

export class AccountRepository implements IAccountRepository {
    login = async (userName: string, password: string): Promise<void> => {
        const loginDto: LoginDTO = { userName, password };
        const response = await apiFetchWithBody(`https://localhost:44347/Account/Login`, 'POST', loginDto);

        if(!response.ok) {
            throw new Error(`Login failed: ${response.status}: ${await response.text()}`);
        }

        return;
    }

    logout = async (): Promise<void> => {
        const response = await apiFetchWithBody(`https://localhost:44347/Account/Logout`, 'POST');

        if(!response.ok) {
            throw new Error(`Login failed: ${response.status}: ${await response.text()}`);
        }

        return;
    }
}
