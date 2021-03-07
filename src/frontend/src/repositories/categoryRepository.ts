import { CategoryDTO } from "src/logic/dtos";
import { Category } from "src/logic/entities";
import { IUserRepository } from "./userRepository";

export interface ICategoryRepository {
    getById(id: number) : Promise<Category>;
};

export class CategoryRepository implements ICategoryRepository {
    userRepo: IUserRepository;

    constructor(userRepository: IUserRepository) {
        this.userRepo = userRepository;
    }

    async getById(id: number): Promise<Category> {
        const response = await fetch(`https://localhost:44347/Category/${id}`, {
            credentials: 'include',
            mode: 'cors',
        });
        if(!response.ok) {
            throw new Error(`${response.status}: ${await response.text()}`)
        }
        
        const categoryDto = await response.json() as CategoryDTO;
        const owner = this.userRepo.getByUserName(categoryDto.ownerUserName);
        const allowedUsers = Promise.all(categoryDto.allowedUserNames.map(userName => this.userRepo.getByUserName(userName)));

        const category: Category = {
            ...categoryDto,
            owner: await owner,
            allowedUsers: await allowedUsers,
        };

        return category;
    }
}