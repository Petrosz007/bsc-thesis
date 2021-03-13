import { CategoryDTO } from "src/logic/dtos";
import { Category } from "src/logic/entities";
import { ResultPromise } from "../utilities/result";
import { IUserRepository } from "./userRepository";
import { safeApiFetch } from "./utilities";

export interface ICategoryRepository {
    getById(id: number): ResultPromise<Category,Error>;
};

export class CategoryRepository implements ICategoryRepository {
    userRepo: IUserRepository;

    constructor(userRepository: IUserRepository) {
        this.userRepo = userRepository;
    }

    getById = (id: number): ResultPromise<Category,Error> =>
        safeApiFetch(`https://localhost:44347/Category/${id}`, 'GET')
            .andThenAsync(async response => {               
                const categoryDto = await response.json() as CategoryDTO;
                const ownerResult = this.userRepo.getByUserName(categoryDto.ownerUserName);
                const atendeeResults = ResultPromise.all(
                        categoryDto.allowedUserNames.map(userName => this.userRepo.getByUserName(userName))
                    );

                return ownerResult
                    .andThenAsync(async owner => 
                        atendeeResults.map(allowedUsers => (
                            {
                                ...categoryDto,
                                owner,
                                allowedUsers,
                            } as Category)
                        )
                    );
            });
}