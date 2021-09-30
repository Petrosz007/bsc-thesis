import UserName from "../UserName";
import Select from "react-select";
import React, {useMemo} from "react";
import {User} from "../../logic/entities";

export default ({ className, users, selectedUser, setSelectedUser }: {
    className?: string,
    users: User[],
    selectedUser: User,
    setSelectedUser: React.Dispatch<React.SetStateAction<User>>,
}) => {
    const selectOptions = useMemo(() => users.map(u => ({ value: u, label: <UserName user={u} /> })), [users]);
    
    return (
        <Select className={className}
                options={selectOptions}
                onChange={e => setSelectedUser(e?.value ?? users[0])}
                filterOption={(option: any, searchText) => `${option.value.name} @${option.value.userName}`.toUpperCase().includes(searchText.toUpperCase())}
                value={{ value: selectedUser, label: <UserName user={selectedUser} /> }}
        />
    );
}