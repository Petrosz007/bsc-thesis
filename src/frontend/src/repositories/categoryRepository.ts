import { CategoryDTO } from "../logic/dtos";
import { Category } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { IUserRepository } from "./userRepository";
import { safeApiFetchAs } from "./utilities";

export interface ICategoryRepository {
    getById(id: number): ResultPromise<Category,Error>;
};

export class CategoryRepository implements ICategoryRepository {
    constructor(
        private readonly userRepo: IUserRepository
    ) {}

    getById = (id: number): ResultPromise<Category,Error> =>
        safeApiFetchAs<CategoryDTO>(`https://localhost:44347/Category/${id}`, 'GET')
            .andThen(categoryDto => {             
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