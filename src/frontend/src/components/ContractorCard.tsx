import {Appointment, User} from "../logic/entities";
import React, {useContext} from "react";

import './ContractorCard.scss';
import { Link } from "react-router-dom";
import {DIContext} from "./contexts/DIContext";

export const ContractorCard = ({ contractor }: { contractor: User }) => {
    const { config } = useContext(DIContext);
    
    return (
        <div className="contractor-card">
            <Link to={`/contractor/${contractor.userName}`} className="contractor-header">{contractor.name}</Link>
            <div className="contractor-description">
                <p>{contractor.contractorPage?.title}</p>
                <p>{contractor.contractorPage?.bio}</p>
                <img src={`${config.apiUrl}/User/Avatar/${contractor.userName}`} alt={`${contractor.name} profilképe`} />
            </div>
        </div>
    );
};