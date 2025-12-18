import React, { useState } from "react";
import { askAi } from "../api/aiApi";
import toast from "react-hot-toast";

export default function AiChat({ onDataReceived }) {
    const [isOpen, setIsOpen] = useState(false);
    const [input, setInput] = useState("");
    const [messages, setMessages] = useState([
        { role: "ai", content: "Përshëndetje! Si mund t'ju ndihmoj me librarinë sot?" }
    ]);
    const [loading, setLoading] = useState(false);

    const handleSend = async () => {
        if (!input.trim()) return;

        const userMsg = { role: "user", content: input };
        setMessages(prev => [...prev, userMsg]);
        setInput("");
        setLoading(true);

        try {
            const results = await askAi(input);
            
            setMessages(prev => [...prev, { 
                role: "ai", 
                content: `Gjeta ${results.length} rezultate për ty.` 
            }]);

            onDataReceived(results);
            
        } catch (err) {
            setMessages(prev => [...prev, { role: "ai", content: "Më vjen keq, pati një gabim gjatë kërkimit." }]);
        } finally {
            setLoading(false);
        }
    };

    if (!isOpen) {
        return (
            <button 
                onClick={() => setIsOpen(true)}
                style={{ position: 'fixed', bottom: '20px', right: '20px', borderRadius: '50%', width: '60px', height: '60px', backgroundColor: '#2196f3', color: 'white', border: 'none', cursor: 'pointer', fontSize: '24px' }}
            >
                🤖
            </button>
        );
    }

    return (
        <div className="ai-chat-window">
            <div className="chat-header">
                AI Library Assistant
                <button onClick={() => setIsOpen(false)} style={{ background: 'none', border: 'none', color: 'white', cursor: 'pointer' }}>X</button>
            </div>
            
            <div className="chat-messages">
                {messages.map((msg, i) => (
                    <div key={i} className={`message ${msg.role}`}>
                        {msg.content}
                    </div>
                ))}
                {loading && <div className="message ai">Duke kërkuar...</div>}
            </div>

            <div className="chat-input-area">
                <input 
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && handleSend()}
                    placeholder="Pyet diçka..."
                />
                <button onClick={handleSend} disabled={loading}>Dërgo</button>
            </div>
        </div>
    );
}