import {Appointment, User} from "../logic/entities";
import React from "react";

import './ContractorCard.scss';
import { Link } from "react-router-dom";

export const ContractorCard = ({ contractor }: { contractor: User }) => {
    return (
        <div className="contractor-card">
            <Link to={`/contractor/${contractor.userName}`} className="contractor-header">{contractor.name}</Link>
            <div className="contractor-description">
                <p>{contractor.contractorPage?.title}</p>
                <p>{contractor.contractorPage?.bio}</p>
                <img src={`https://localhost:5001/User/Avatar/${contractor.userName}`} alt={`Avatar of ${contractor.name}`} />
            </div>
        </div>
    );
};