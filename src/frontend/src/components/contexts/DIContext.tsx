import React from "react";
import { createContext } from "react";
import { AccountRepository, IAccountRepository } from "../../repositories/accountRepository";
import { AppointmentRepository, IAppointmentRepository } from "../../repositories/appointmentRepository";
import { CategoryRepository, ICategoryRepository } from "../../repositories/categoryRepository";
import { IUserRepository, UserRepository } from "../../repositories/userRepository";

export interface DIContextState {
    userRepo: IUserRepository;
    appointmentRepo: IAppointmentRepository;
    categoryRepo: ICategoryRepository;
    accountRepo: IAccountRepository;
}

class InitialState implements DIContextState {
    constructor() {
        this.userRepo = new UserRepository();
        this.categoryRepo = new CategoryRepository(this.userRepo);
        this.appointmentRepo = new AppointmentRepository(this.userRepo, this.categoryRepo);
        this.accountRepo = new AccountRepository();
    }

    userRepo: IUserRepository;
    appointmentRepo: IAppointmentRepository;
    categoryRepo: ICategoryRepository;
    accountRepo: IAccountRepository;
}

export const DIContext = createContext<DIContextState>(new InitialState());
