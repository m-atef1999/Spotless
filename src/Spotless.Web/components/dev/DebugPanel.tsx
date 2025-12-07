import React, { useState, useEffect } from 'react';
import { useAuthStore } from '../../store/authStore';
import { ChevronDown, ChevronUp, Terminal, RefreshCw, Trash2 } from 'lucide-react';

// Toggle debug panel via: localStorage.setItem('debug-panel-enabled', 'true')
// Or in console: localStorage.setItem('debug-panel-enabled', 'true'); location.reload();
// To disable: localStorage.removeItem('debug-panel-enabled'); location.reload();

export const DebugPanel: React.FC = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [isEnabled, setIsEnabled] = useState(false);
    const { user, role, token } = useAuthStore();
    const [storageContent, setStorageContent] = useState<string>('');

    useEffect(() => {
        // Check if debug panel is enabled via localStorage
        const enabled = localStorage.getItem('debug-panel-enabled') === 'true';
        setIsEnabled(enabled);
    }, []);

    const updateStorageContent = () => {
        const content = localStorage.getItem('auth-storage');
        try {
            setStorageContent(content ? JSON.stringify(JSON.parse(content), null, 2) : 'null');
        } catch {
            setStorageContent(content || 'null');
        }
    };

    useEffect(() => {
        if (!isEnabled) return;
        updateStorageContent();
        const interval = setInterval(updateStorageContent, 1000);
        return () => clearInterval(interval);
    }, [isEnabled]);

    const clearStorage = () => {
        localStorage.removeItem('auth-storage');
        updateStorageContent();
        window.location.reload();
    };

    // Only show if explicitly enabled via localStorage
    if (!isEnabled) return null;

    return (
        <div className={`fixed bottom-24 right-6 z-50 transition-all duration-300 ${isOpen ? 'w-96' : 'w-auto'}`}>
            <div className="bg-slate-900 text-slate-200 rounded-lg shadow-2xl border border-slate-700 overflow-hidden">
                <div
                    className="flex items-center justify-between p-3 bg-slate-800 cursor-pointer hover:bg-slate-700 transition-colors"
                    onClick={() => setIsOpen(!isOpen)}
                >
                    <div className="flex items-center gap-2 font-mono text-sm font-bold text-cyan-400">
                        <Terminal className="w-4 h-4" />
                        <span>Debug Panel</span>
                    </div>
                    {isOpen ? <ChevronDown className="w-4 h-4" /> : <ChevronUp className="w-4 h-4" />}
                </div>

                {isOpen && (
                    <div className="p-4 space-y-4 max-h-[80vh] overflow-y-auto font-mono text-xs">
                        {/* Auth State */}
                        <div className="space-y-2">
                            <h3 className="font-bold text-slate-400 uppercase tracking-wider">Auth State</h3>
                            <div className="bg-slate-950 p-3 rounded border border-slate-800 space-y-1">
                                <div className="flex justify-between">
                                    <span className="text-slate-500">Role:</span>
                                    <span className="text-green-400">{role || 'null'}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-slate-500">Token:</span>
                                    <span className={token ? "text-green-400" : "text-red-400"}>
                                        {token ? 'Present' : 'Missing'}
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-slate-500">User:</span>
                                    <span className={user ? "text-green-400" : "text-yellow-400"}>
                                        {user ? 'Loaded' : 'Null'}
                                    </span>
                                </div>
                            </div>
                        </div>

                        {/* User Details */}
                        {user && (
                            <div className="space-y-2">
                                <h3 className="font-bold text-slate-400 uppercase tracking-wider">User Details</h3>
                                <pre className="bg-slate-950 p-3 rounded border border-slate-800 overflow-x-auto text-blue-300">
                                    {JSON.stringify(user, null, 2)}
                                </pre>
                            </div>
                        )}

                        {/* Local Storage */}
                        <div className="space-y-2">
                            <div className="flex items-center justify-between">
                                <h3 className="font-bold text-slate-400 uppercase tracking-wider">Local Storage</h3>
                                <div className="flex gap-2">
                                    <button onClick={updateStorageContent} className="p-1 hover:text-cyan-400" title="Refresh">
                                        <RefreshCw className="w-3 h-3" />
                                    </button>
                                    <button onClick={clearStorage} className="p-1 hover:text-red-400" title="Clear & Reload">
                                        <Trash2 className="w-3 h-3" />
                                    </button>
                                </div>
                            </div>
                            <pre className="bg-slate-950 p-3 rounded border border-slate-800 overflow-x-auto text-slate-400 max-h-40">
                                {storageContent}
                            </pre>
                        </div>

                        {/* Environment */}
                        <div className="space-y-2">
                            <h3 className="font-bold text-slate-400 uppercase tracking-wider">Environment</h3>
                            <div className="bg-slate-950 p-3 rounded border border-slate-800 space-y-1">
                                <div className="flex justify-between">
                                    <span className="text-slate-500">Mode:</span>
                                    <span className="text-yellow-400">{import.meta.env.MODE}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-slate-500">API URL:</span>
                                    <span className="text-slate-300 truncate max-w-[150px]" title={import.meta.env.VITE_API_URL}>
                                        {import.meta.env.VITE_API_URL}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};
