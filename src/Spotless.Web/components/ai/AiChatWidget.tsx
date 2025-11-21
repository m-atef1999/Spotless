import React, { useState, useRef, useEffect } from 'react';
import { MessageSquare, X, Send, Loader2, Bot } from 'lucide-react';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useToast } from '../ui/Toast';
import { AiService } from '../../lib/services/AiService';

interface Message {
    id: string;
    text: string;
    sender: 'user' | 'ai';
    timestamp: Date;
}

export const AiChatWidget: React.FC = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState<Message[]>([
        {
            id: '1',
            text: "Hi! I'm the Spotless Smart Assistant. Ask me about our services or prices!",
            sender: 'ai',
            timestamp: new Date()
        }
    ]);
    const [inputValue, setInputValue] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { addToast } = useToast();

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, isOpen]);

    const handleSendMessage = async () => {
        if (!inputValue.trim()) return;

        const userMessage: Message = {
            id: Date.now().toString(),
            text: inputValue,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, userMessage]);
        setInputValue('');
        setIsLoading(true);

        try {
            const response = await AiService.postApiAiChat({
                requestBody: {
                    message: userMessage.text
                }
            });

            const aiMessage: Message = {
                id: (Date.now() + 1).toString(),
                text: response.response || "I'm sorry, I couldn't process that.",
                sender: 'ai',
                timestamp: new Date()
            };

            setMessages(prev => [...prev, aiMessage]);
        } catch (error) {
            console.error('Failed to send message:', error);
            addToast('Failed to get response from AI', 'error');

            // Add error message to chat
            setMessages(prev => [...prev, {
                id: (Date.now() + 1).toString(),
                text: "Sorry, I'm having trouble connecting right now. Please try again later.",
                sender: 'ai',
                timestamp: new Date()
            }]);
        } finally {
            setIsLoading(false);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    return (
        <div className="fixed bottom-6 right-6 z-[100] flex flex-col items-end">
            {/* Chat Window */}
            {isOpen && (
                <div className="mb-4 w-80 md:w-96 bg-white dark:bg-slate-900 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-800 overflow-hidden flex flex-col transition-all duration-300 animate-in slide-in-from-bottom-10 fade-in">
                    {/* Header */}
                    <div className="p-4 bg-gradient-to-r from-cyan-500 to-blue-600 text-white flex justify-between items-center">
                        <div className="flex items-center gap-2">
                            <div className="p-1.5 bg-white/20 rounded-lg backdrop-blur-sm">
                                <Bot className="w-5 h-5" />
                            </div>
                            <div>
                                <h3 className="font-bold text-sm">Spotless Assistant</h3>
                                <p className="text-xs text-cyan-100 flex items-center gap-1">
                                    <span className="w-1.5 h-1.5 bg-green-400 rounded-full animate-pulse" />
                                    Online
                                </p>
                            </div>
                        </div>
                        <button
                            onClick={() => setIsOpen(false)}
                            className="p-1 hover:bg-white/20 rounded-full transition-colors"
                        >
                            <X className="w-5 h-5" />
                        </button>
                    </div>

                    {/* Messages Area */}
                    <div className="h-96 overflow-y-auto p-4 space-y-4 bg-slate-50 dark:bg-slate-950/50">
                        {messages.map((msg) => (
                            <div
                                key={msg.id}
                                className={`flex ${msg.sender === 'user' ? 'justify-end' : 'justify-start'}`}
                            >
                                <div
                                    className={`max-w-[80%] p-3 rounded-2xl text-sm ${msg.sender === 'user'
                                        ? 'bg-cyan-500 text-white rounded-br-none'
                                        : 'bg-white dark:bg-slate-800 text-slate-800 dark:text-slate-200 border border-slate-200 dark:border-slate-700 rounded-bl-none shadow-sm'
                                        }`}
                                >
                                    {msg.text.split('\n').map((line, i) => (
                                        <p key={i} className={i > 0 ? 'mt-1' : ''}>{line}</p>
                                    ))}
                                    <p className={`text-[10px] mt-1 ${msg.sender === 'user' ? 'text-cyan-100' : 'text-slate-400'
                                        }`}>
                                        {msg.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                    </p>
                                </div>
                            </div>
                        ))}
                        {isLoading && (
                            <div className="flex justify-start">
                                <div className="bg-white dark:bg-slate-800 p-3 rounded-2xl rounded-bl-none border border-slate-200 dark:border-slate-700 shadow-sm">
                                    <Loader2 className="w-4 h-4 animate-spin text-cyan-500" />
                                </div>
                            </div>
                        )}
                        <div ref={messagesEndRef} />
                    </div>

                    {/* Input Area */}
                    <div className="p-4 bg-white dark:bg-slate-900 border-t border-slate-100 dark:border-slate-800">
                        <div className="flex items-center gap-2 w-full">
                            <div className="relative flex-1">
                                <Input
                                    value={inputValue}
                                    onChange={(e) => setInputValue(e.target.value)}
                                    onKeyPress={handleKeyPress}
                                    placeholder="Type a message..."
                                    className="w-full pr-2"
                                    disabled={isLoading}
                                />
                            </div>
                            <Button
                                onClick={handleSendMessage}
                                disabled={!inputValue.trim() || isLoading}
                                size="sm"
                                className="h-10 w-10 shrink-0 rounded-xl bg-cyan-500 hover:bg-cyan-600 text-white shadow-md p-0 flex items-center justify-center"
                            >
                                {isLoading ? (
                                    <Loader2 className="w-4 h-4 animate-spin" />
                                ) : (
                                    <Send className="w-4 h-4" />
                                )}
                            </Button>
                        </div>
                    </div>
                </div>
            )}

            {/* Toggle Button */}
            <button
                onClick={() => setIsOpen(!isOpen)}
                className={`p-4 rounded-full shadow-lg transition-all duration-300 hover:scale-110 active:scale-95 ${isOpen
                    ? 'bg-slate-200 text-slate-600 dark:bg-slate-800 dark:text-slate-400'
                    : 'bg-gradient-to-r from-cyan-500 to-blue-600 text-white animate-bounce-subtle'
                    }`}
            >
                {isOpen ? (
                    <X className="w-6 h-6" />
                ) : (
                    <MessageSquare className="w-6 h-6" />
                )}
            </button>
        </div>
    );
};
