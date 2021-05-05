import UserName from "../UserName";
import Select from "react-select";
import React from "react";
import {User} from "../../logic/entities";

export default ({ className, users, selectedUser, setSelectedUser }: {
    className?: string,
    users: User[],
    selectedUser: User,
    setSelectedUser: React.Dispatch<React.SetStateAction<User>>,
}) => {
    return (
        <Select className={className}
                options={users.map(u => ({ value: u, label: <UserName user={u} /> }))}
                onChange={e => setSelectedUser(e?.value ?? users[0])}
                filterOption={(option: any, searchText) => `${option.value.name} @${option.value.userName}`.toUpperCase().includes(searchText.toUpperCase())}
                value={{ value: selectedUser, label: <UserName user={selectedUser} /> }}
        />
    );
}