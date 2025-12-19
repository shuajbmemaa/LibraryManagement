import React from "react";
import { Button } from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";

export const userColumns = (handleEdit, handleDelete) => [
    {
        title: 'Name',
        dataIndex: 'name',
        key: 'name',
    },
    {
        title: 'Email',
        dataIndex: 'email',
        key: 'email',
    },
    {
        title: 'Role',
        dataIndex: 'roles',
        key: 'roles',
        render: (role) => (role?.length > 0 ? role[0] : '-'),
    },
    {
        title: 'Actions',
        dataIndex: 'actions',
        key: 'actions',
        width: 100,
        fixed: 'right',
        render: (text, record) => (
            <div className="action-wrapper">
                <Button type="primary" danger onClick={() => handleDelete(record)}> <DeleteOutlined /> </Button>
            </div>
        ),
    }
];