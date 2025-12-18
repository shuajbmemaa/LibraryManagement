import { Modal, Form, Input, Select, Space, Button } from "antd";
import React, {useState} from "react";
import { createAdminUser } from "../api/userApi";


function AddUserModal ({open, onCancel, finished}) {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);

    const onFinish = (values) => {
        setLoading(true);
        createAdminUser(values).then(() => {
            form.resetFields();
            finished();
            setLoading(false);
        }).catch((error) => {
            console.error("Error creating user:", error);
            setLoading(false);
        });
    }
    return (
        <Modal title="Add User" open={open} footer={null} onCancel={() => onCancel()}>
            <Form
                // {...layout}
                labelCol={{ span: 6 }}
                wrapperCol={{ span: 14 }}
                layout="horizontal"
                autoComplete={false}
                form={form}
                name="control-hooks"
                onFinish={onFinish}
                style={{ maxWidth: 600, marginTop: '30px' }}
                >
                <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="email" label="Email" rules={[{ required: true, type: 'email' }]}>
                    <Input type={'email'}/>
                </Form.Item>
                <Form.Item name="password" label="Password" rules={[{ required: true, type: 'password'}]}>
                    <Input type={'password'}/>
                </Form.Item>
                <Form.Item name="role" label="Role" rules={[{ required: true }]}>
                    <Select
                    allowClear
                    placeholder="Select a role"
                    // onChange={onGenderChange}
                    options={[
                        { label: 'admin', value: 'admin' },
                        { label: 'user', value: 'user' },
                    ]}
                    />
                </Form.Item>
                <Form.Item wrapperCol={{ span: 14, offset: 10 }}>
                    <Space >
                        <Button type="primary" htmlType="submit" loading={loading}>
                            Submit
                        </Button>
                    </Space>
                </Form.Item>
            </Form>
        </Modal>
    )
}

export default AddUserModal;