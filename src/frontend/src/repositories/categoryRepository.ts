import { CategoryDTO } from "../logic/dtos";
import { Category } from "../logic/entities";
import { ResultPromise } from "../utilities/result";
import { IUserRepository } from "./userRepository";
import { safeApiFetchAs } from "./utilities";

export interface ICategoryRepository {
    getById(id: number): ResultPromise<Category,Error>;
    getContractorsCategories(username: string): ResultPromise<Category[],Error>;
};

export class CategoryRepository implements ICategoryRepository {
    constructor(
        private readonly userRepo: IUserRepository
    ) {}

    private dtoToEntity = (dto: CategoryDTO): ResultPromise<Category,Error> => 
        ResultPromise.ok<CategoryDTO,Error>(dto)
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
    
    private dtosToEntities = (dtos: CategoryDTO[]): ResultPromise<Category[],Error> =>
            ResultPromise.ok<CategoryDTO[],Error>(dtos)
                .andThen(categoryDtos => ResultPromise.all(categoryDtos.map(this.dtoToEntity)));

    getById = (id: number): ResultPromise<Category,Error> =>
        safeApiFetchAs<CategoryDTO>(`https://localhost:44347/Category/${id}`, 'GET')
            .andThen(this.dtoToEntity);

    getContractorsCategories = (userName: string): ResultPromise<Category[],Error> =>
        safeApiFetchAs<CategoryDTO[]>(`https://localhost:44347/Category/Contractor/${userName}`, 'GET')
            .andThen(this.dtosToEntities);
}