import {User} from "../logic/entities";
import React, {useContext} from "react";

import './ContractorCard.scss';
import { Link } from "react-router-dom";
import {DIContext} from "./contexts/DIContext";

export const ContractorCard = ({ contractor }: { contractor: User }) => {
    const { config } = useContext(DIContext);
    
    return (
        <div className="contractor-card">
            <img src={`${config.apiUrl}/User/Avatar/${contractor.userName}`} alt={`${contractor.name} profilképe`} />
            <div className="contractorInfo">
                <Link to={`/contractor/${contractor.userName}`} className="contractor-header">{contractor.name}</Link> - <span className="contractor-description-title">{contractor.contractorPage?.title}</span>
                <p className="contractor-description-bio">{contractor.contractorPage?.bio}</p>
            </div>
        </div>
    );
};