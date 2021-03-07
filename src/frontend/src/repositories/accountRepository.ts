import { LoginDTO } from "../logic/dtos";

export interface IAccountRepository {
    login(userName: string, password: string): Promise<void>;
    logout(): Promise<void>;
}

export class AccountRepository implements IAccountRepository {
    login = async (userName: string, password: string): Promise<void> => {
        const loginDto: LoginDTO = { userName, password };
        const response = await fetch(`https://localhost:44347/Account/Login`, {
            method: 'POST',
            credentials: 'include',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginDto),
        });

        if(!response.ok) {
            throw new Error(`Login failed: ${response.status}: ${await response.text()}`);
        }

        return;
    }

    logout = async (): Promise<void> => {
        const response = await fetch(`https://localhost:44347/Account/Logout`, {
            method: 'POST',
            credentials: 'include',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json',
            },
            body: '',
        });

        if(!response.ok) {
            throw new Error(`Login failed: ${response.status}: ${await response.text()}`);
        }

        return;
    }
}
