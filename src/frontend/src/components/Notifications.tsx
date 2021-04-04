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
            <button onClick={() => notificationDispatch({ type: 'remove', id: notification.id })}>X</button>
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