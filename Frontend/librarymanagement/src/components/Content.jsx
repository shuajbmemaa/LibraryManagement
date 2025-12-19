import { Table, Spin, Input, Modal, Button} from "antd";
import React, { useEffect, useState } from "react";
import { bookColumns } from "../staticStructures/bookColumns";
import { userColumns } from "../staticStructures/userColumns";
import { getAllBooks, deleteBook } from "../api/bookApi";
import { getAllUsers, deleteUser} from "../api/userApi";
import { PlusOutlined } from "@ant-design/icons";
import AddUserModal from "../modals/AddUserModal";
import AddBookModal from "../modals/AddBookModal";
import EditBookModal from "../modals/EditBookModal";
import Filters from "./Filters";
import "../design/Table.scss";

export function Content ({active}) {
    const [activeColumns, setActiveColumns] = useState([]);
    const [data, setData] = useState([]);
    const [dataToEdit, setDataToEdit] = useState({});
    const [loading, setLoading] = useState(false);
    const [isAddUserModalOpen, setIsAddUserModalOpen] = useState(false);
    const [isAddBookModalOpen, setIsAddBookModalOpen] = useState(false);
    const [isEditBookModalOpen, setIsEditBookModalOpen] = useState(false);

    const handleEdit = (record) => {
        setIsEditBookModalOpen(true);
        setDataToEdit(record);
    }

    const handleDelete = (record) => {
        Modal.warning({
            okText: 'Yes',
            cancelText: 'No',
            closable: true,
            cancelButtonProps: { type: 'default' },
            title: `Are you sure you want to delete ${record?.name ? record.name : record?.title }?`,
            onOk() {
                setLoading(true);
                if (active === 'books') {
                    deleteBook(record.id).then(() => {
                        setData((prevData) => prevData.filter((item) => item.id !== record.id));
                        setLoading(false);
                    }).catch(() => {
                        setLoading(false);
                    });
                }else if (active === 'users') {
                    deleteUser(record.id).then(() => {
                        setData((prevData) => prevData.filter((item) => item.id !== record.id));
                        setLoading(false);
                    }).catch(() => {
                        setLoading(false);
                    });
                }
            },
            centered: true,
            okCancel: true,
            icon: null,
        });
    }
    const allBooks = bookColumns(handleEdit, handleDelete);
    const allUsers = userColumns(handleEdit, handleDelete);

    const fetchData = (filters) => {
        setLoading(true);
        setData([]);
        if (active === 'books') {            
            setActiveColumns(allBooks);
            getAllBooks(filters || {}).then(
            (books) => {
                setLoading(false);
                setData(books?.map((book) => ({...book, key: book.id})));
            }).catch(() => {
                setLoading(false);
            });
        } else if (active === 'users') {
            setActiveColumns(allUsers);
            getAllUsers(filters || {}).then((users) => {
                setLoading(false);
                setData(users?.map((user) => ({...user, key: user.id})));
            }).catch(() => {
                setLoading(false);
            });
        }
    }

    const filterData = (filters) => {
        fetchData(filters);       
    }

    useEffect(() => {
        fetchData();
    }, [active]);

    return (
        <>
            {<AddUserModal open={isAddUserModalOpen} onCancel={() => setIsAddUserModalOpen(false)} finished={() => {fetchData(); setIsAddUserModalOpen(false);}}/>}
            {<AddBookModal open={isAddBookModalOpen} onCancel={() => setIsAddBookModalOpen(false)} finished={() => {fetchData(); setIsAddBookModalOpen(false);}}/>}
            {<EditBookModal dataToEdit={dataToEdit} open={isEditBookModalOpen} onCancel={() => setIsEditBookModalOpen(false)} finished={() => {fetchData(); setIsEditBookModalOpen(false);}}/>}
            {active === 'books' ? <h2> Books </h2> :  active === 'users' ? <h2> Users </h2> : <></>}
            <div className="filters">
                {active === 'books' ? <Filters filterData={(data) => filterData(data)} /> : <div></div>}
                {active === 'users' ? (
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => {setIsAddUserModalOpen(true);}}>
                        Add User
                    </Button>
                ) : active === 'books' ? (
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => {setIsAddBookModalOpen(true);}}>
                        Add Book
                    </Button>
                ) : null}
            </div>
            <div className="table">
                <Table
                    columns={activeColumns}
                    dataSource={data}
                    loading={loading}
                />
            </div>
        </>
    );
}