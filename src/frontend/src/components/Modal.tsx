import React, {useEffect, useRef} from "react";

import './Modal.scss';

export default ({children, className, isOpen = true}: {
    isOpen?: boolean,
    children: React.ReactNode,
    className?: string,
}) => {
    const divRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        setTimeout(() => {
            if (isOpen) {
                divRef.current?.classList.add('visible');
            } else {
                divRef.current?.classList.remove('visible');
            }
        }, 10);
    }, [isOpen]);

    return isOpen
        ?
        <div ref={divRef} className="modal-wrapper">
            <div className={`modal-content ${className ?? ''}`}>
                {children}
            </div>
        </div>
        : null;
}