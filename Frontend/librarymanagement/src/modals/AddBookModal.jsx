import { Modal, Form, Input, Select, Space, Button } from "antd";
import React, {useState} from "react";
import { createBook } from "../api/bookApi";


function AddBookModal ({open, onCancel, finished}) {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);

    const onFinish = (values) => {
        setLoading(true);
        createBook(values).then(() => {
            finished();
            form.resetFields();
            setLoading(false);
        }).catch((error) => {
            console.error("Error creating book:", error);
            setLoading(false);
        });
    }
    return (
        <Modal title="Add Book" open={open} footer={null} onCancel={() => onCancel()}>
            <Form
                // {...layout}
                labelCol={{ span: 6 }}
                wrapperCol={{ span: 14 }}
                layout="horizontal"
                form={form}
                name="control-hooks"
                onFinish={onFinish}
                style={{ maxWidth: 600, marginTop: '30px' }}
                >
                <Form.Item name="title" label="Title" rules={[{ required: true }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="author" label="Author" rules={[{ required: true }]}>
                    <Input/>
                </Form.Item>
                <Form.Item name="genre" label="Genre" rules={[{ required: true }]}>
                    <Input/>
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

export default AddBookModal;