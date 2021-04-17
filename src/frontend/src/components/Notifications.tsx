import React, { useContext, useEffect, useRef } from "react"
import { NotificationContext, NotificationData } from "./contexts/NotificationProvider";

import './Notifications.scss';

const Notification = ({ notification }: { notification: NotificationData }) => {
    const { notificationDispatch } = useContext(NotificationContext);
    const divRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        setTimeout(() => {
            divRef.current?.classList.toggle('visible');
        }, 10);
    }, []);

    return (
        <div ref={divRef} className={`notification notification-${notification.notificationType}`}>
            <p>{notification.message}</p>
            <svg height="330pt" viewBox="0 0 330 330" width="330pt"
                 onClick={() => notificationDispatch({ type: 'remove', id: notification.id })}
            >
                <path d="m195 
                165 128.210938-128c8.34375-8.339844 
                8.34375-21.824219 0-30.164063-8.339844-8.339844-21.824219-8.339844-30.164063 
                0l-128 128-128.210937-128c-8.34375-8.339844-21.824219-8.339844-30.164063 0-8.34375
                8.339844-8.34375 21.824219 0 30.164063l128 128-128 
                128c-8.34375 8.339844-8.34375 21.824219 0 30.164063 4.15625 4.160156 9.621094 6.25 15.082032 
                6.25 5.460937 0 10.921875-2.089844 15.082031-6.25l128.210937-128 128 128c4.160156 
                4.160156 9.621094 6.25 15.082032 6.25 5.460937 0 
                10.921874-2.089844 15.082031-6.25 8.34375-8.339844 
                8.34375-21.824219 0-30.164063zm0 0"/>
            </svg>
        </div>
    );
}

export default ({ notifications }: { notifications: NotificationData[] }) => {
    return (
        <div className="notifications">
            {notifications.map(n => <Notification notification={n} key={n.id}/>)}
        </div>
    )
}