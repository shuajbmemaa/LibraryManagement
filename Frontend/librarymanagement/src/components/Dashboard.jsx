import React, { useEffect, useState, useCallback } from "react";
import { getAllBooks, deleteBook } from "../api/bookApi";
import { getAllUsers, deleteUser } from "../api/userApi";
import { askAi } from "../api/aiApi";
import useLocalStorage from "use-local-storage";
import toast, { Toaster } from "react-hot-toast";
import { useNavigate } from "react-router-dom";
import {Breadcrumb, Button, Layout, Menu, theme} from 'antd';
import {UserOutlined, BookOutlined} from '@ant-design/icons'
import {Content as AllContent} from "./Content";

const { Header, Content, Sider } = Layout;

export default function Dashboard() {
    const [user, setUser] = useLocalStorage('user');
    const allMenuItems  = [
        ...(user?.isAdmin ? [{key: 'users', label: 'Users', icon: <UserOutlined/>}] : []),
        {key: 'books', label: 'Books', icon: <BookOutlined/>},
    ]
    const [view, setView] = useState(allMenuItems[0].key);

    const navigate = useNavigate();
    const {
        token: { colorBgContainer, borderRadiusLG },
    } = theme.useToken();


    const [activeMenu, setActiveMenu] = useState(allMenuItems[0].key)

    const handleLogout = () => {
        setUser(null);
        navigate("/");
    };

    useEffect(() => {
        console.log(activeMenu);
    }, [activeMenu])

    return (
        <>
        <Layout style={{minHeight: '100vh'}}>
            <Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', color: '#fff', paddingLeft: 20}}>
                <h3>Library Management System</h3>
                <div className="logout-section">
                    <Button type="primary" danger onClick={handleLogout}>
                        Logout
                    </Button>
                </div>
            </Header>
            <Layout>
                <Sider width={200} style={{ background: colorBgContainer }}>
                <Menu
                    mode="inline"
                    defaultSelectedKeys={[activeMenu]}
                    style={{ height: '100%', borderInlineEnd: 0 }}
                    items={allMenuItems}
                    onClick={(item, b) => setActiveMenu(item.key)}
                />
                </Sider>
                <Layout style={{ padding: '0 24px 24px' }}>
                <Breadcrumb
                    items={[{ title: 'Dashboard' }, { title: allMenuItems.find(i => i.key === activeMenu)?.label }]}
                    style={{ margin: '16px 0' }}
                />
                <Content
                    style={{
                    padding: 24,
                    margin: 0,
                    minHeight: 280,
                    background: colorBgContainer,
                    borderRadius: borderRadiusLG,
                    }}
                >
                    <AllContent active={activeMenu} />
                </Content>
                </Layout>
            </Layout>
        </Layout>
        </>
    );
}