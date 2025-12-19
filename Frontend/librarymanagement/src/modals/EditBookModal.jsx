import { Modal, Form, Input, Select, Space, Button } from "antd";
import React, {useEffect, useState} from "react";
import { updateBook } from "../api/bookApi";
import { bookStatusEnum } from "../staticStructures/bookStatus";


function EditBookModal ({dataToEdit, open, onCancel, finished}) {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        console.log(dataToEdit);
        if (dataToEdit) {
            form.setFieldsValue({
                title: dataToEdit?.title,
                author: dataToEdit?.author,
                genre: dataToEdit?.genre,
                readingStatus: {label: bookStatusEnum[dataToEdit?.readingStatus], value: dataToEdit?.readingStatus}
            });
        }
    }, [dataToEdit])
    const onFinish = (values) => {
        setLoading(true);
        const formattedValues = {
            ...values,
            readingStatus: parseInt(values.readingStatus)
        };
        updateBook(dataToEdit?.id, formattedValues).then(() => {
            finished();
            setLoading(false);
        }).catch((error) => {
            console.error("Error updating book:", error);
            setLoading(false);
        });
    }
    return (
        <Modal title="Edit Book" open={open} footer={null} onCancel={() => onCancel()} destroyOnHidden={true} forceRender>
            <Form
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
                    <Input />
                </Form.Item>
                <Form.Item name="genre" label="Genre" rules={[{ required: true }]}>
                    <Input/>
                </Form.Item>
                <Form.Item name="readingStatus" label="Status" rules={[{ required: true }]}>
                    <Select
                        options={
                            Object.keys(bookStatusEnum).map((key) => ({ label: bookStatusEnum[key], value: parseInt(key) }))
                        }
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

export default EditBookModal;