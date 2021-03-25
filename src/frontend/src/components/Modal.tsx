import React from "react";

import './Modal.scss';

export default ({ children, className }: { 
    children: React.ReactNode,
    className?: string,
}) => {
    return (
        <div className="modal-wrapper">
            <div className={`modal-content ${className ?? ''}`}>
                {children}
            </div>
        </div>
    );
}