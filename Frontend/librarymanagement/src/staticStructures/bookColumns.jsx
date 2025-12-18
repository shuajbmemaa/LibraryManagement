import React from "react";
import { Button } from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";
import { bookStatusEnum } from "./bookStatus";

export const bookColumns = (handleEdit, handleDelete) => [
    {
        title: 'Title',
        dataIndex: 'title',
        key: 'title',
    },
    {
        title: 'Author',
        dataIndex: 'author',
        key: 'author',
    },
    {
        title: 'Genre',
        dataIndex: 'genre',
        key: 'genre',
    },
    {
        title: 'Status',
        dataIndex: 'readingStatus',
        key: 'readingStatus',
        render: (status) => (bookStatusEnum[status] || 'Unknown'),
    },
    {
        title: 'Actions',
        key: 'actions',
        width: 100,
        render: (text, record) => (
            <div className="action-wrapper">
                <Button type="primary" onClick={() => handleEdit(record)}> <EditOutlined /> </Button>
                <Button type="primary" danger onClick={() => handleDelete(record)}> <DeleteOutlined /> </Button>
            </div>
        ),
    }
];