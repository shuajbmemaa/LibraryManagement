import React, { useEffect, useState, useRef } from "react";
import { OpenAIOutlined, SearchOutlined } from "@ant-design/icons";
import { Input, Form, Button, Spin } from "antd";
import './AiModule.scss'
import { askAi } from "../api/aiApi";

export function AIModule() {
    const [form] = Form.useForm();
    const [convo, setConvo] = useState([]);
    const [open, setOpen] = useState(false);
    const ref = useRef();
    const [questionRefLoader, setQuestionRefLoader] = useState([]);

    const callAI = (values) => {
        const tempKey = Date.now();
        setConvo((prev) => [...prev, { role: "user", content: values.message, key: tempKey }]);
        setQuestionRefLoader(prev => [...prev, tempKey]);
        form.resetFields();
        askAi(values.message).then((response) => {
            console.log('AI response: ', response);
            setQuestionRefLoader(prev => prev.filter(key => key !== tempKey));
            setConvo((prev) => [...prev, { role: "ai", content: response.answer }]);
        }).catch((error) => {
            console.error("Error communicating with AI:", error);
            setConvo((prev) => [...prev, { role: "ai", content: "Sorry, the request wasn't properly processed." }]);
            setQuestionRefLoader(prev => prev.filter(key => key !== tempKey));
        });
    }
    
    useEffect(() => {
        console.log('convo updated: ', convo);
        if(ref){
            ref.current.scrollTop = ref.current.scrollHeight;
        }
    },[convo])
    return (
        <>
            <div className="ai-wrapper">
                <div className="ai-icon" onClick={() => setOpen((prev) => !prev)}>
                    <OpenAIOutlined style={{ fontSize: '32px', color: '#fff' }} />
                </div>
                <div className="ai-content" style={{display: open ? 'flex' : 'none'}}>
                    <Form form={form} onFinish={callAI} className="ai-form">
                        <Form.Item name="message">
                            <Input.TextArea
                                placeholder="Type your message to the AI assistant..."
                                onPressEnter={form.submit}
                            />
                        </Form.Item>
                        <Form.Item className="ai-search-button">
                            <Button type="primary" htmlType="submit">
                                <SearchOutlined />
                            </Button>
                        </Form.Item>

                    </Form>
                    <div className="conversation-wrapper">
                        <div className="conversation" ref={ref}>
                            {convo.map((msg, index) => (
                                <div className={`message-wrapper ${msg.role}`}>
                                    <div key={index} className={`message`}>
                                        {msg.content}
                                    </div>
                                    {msg.role === 'user' && questionRefLoader.includes(msg.key) && (
                                        <Spin />
                                    )}
                                </div>

                            ))}
                        </div>
                        
                    </div>
                </div>
            </div>
        </>
    )
}