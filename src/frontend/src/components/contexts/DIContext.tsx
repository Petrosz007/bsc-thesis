import React from "react";
import { createContext } from "react";
import { AccountRepository, IAccountRepository } from "../../repositories/accountRepository";
import { AppointmentRepository, IAppointmentRepository } from "../../repositories/appointmentRepository";
import { CategoryRepository, ICategoryRepository } from "../../repositories/categoryRepository";
import { IUserRepository, UserRepository } from "../../repositories/userRepository";
import {Config} from "../../config";

export interface DIContextState {
    config: Config;
    userRepo: IUserRepository;
    appointmentRepo: IAppointmentRepository;
    categoryRepo: ICategoryRepository;
    accountRepo: IAccountRepository;
}

class InitialState implements DIContextState {
    config: Config = new Config();
    userRepo: IUserRepository = new UserRepository(this.config);
    accountRepo: IAccountRepository = new AccountRepository(this.config);
    categoryRepo: ICategoryRepository = new CategoryRepository(this.config, this.userRepo);
    appointmentRepo: IAppointmentRepository = new AppointmentRepository(this.config, this.userRepo, this.categoryRepo);
}

export const DIContext = createContext<DIContextState>(new InitialState());
