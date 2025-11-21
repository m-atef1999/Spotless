import React, { useState, useEffect } from 'react';
import { AuditLogsService, type AuditLogDto, type PagedResponse } from '../../lib/api';

const AuditLogsPage: React.FC = () => {
    const [logs, setLogs] = useState<AuditLogDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [filters, setFilters] = useState({
        userId: '',
        eventType: '',
        startDate: '',
        endDate: ''
    });
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const pageSize = 20;

    useEffect(() => {
        fetchLogs();
    }, [currentPage, filters]);

    const fetchLogs = async () => {
        setLoading(true);
        try {
            const response: PagedResponse = await AuditLogsService.getApiAuditLogs({
                userId: filters.userId || undefined,
                eventType: filters.eventType || undefined,
                startDate: filters.startDate || undefined,
                endDate: filters.endDate || undefined,
                pageNumber: currentPage,
                pageSize
            });

            const auditLogs = (response.data as AuditLogDto[]) || [];
            setLogs(auditLogs);
            setTotalPages(response.totalPages || 1);
        } catch (error) {
            console.error('Failed to fetch audit logs:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleFilterChange = (key: keyof typeof filters, value: string) => {
        setFilters(prev => ({ ...prev, [key]: value }));
        setCurrentPage(1);
    };

    const clearFilters = () => {
        setFilters({
            userId: '',
            eventType: '',
            startDate: '',
            endDate: ''
        });
        setCurrentPage(1);
    };

    const getActionBadgeColor = (eventType: string | null | undefined) => {
        if (!eventType) return 'bg-gray-100 text-gray-800';
        switch (eventType.toUpperCase()) {
            case 'CREATE': return 'bg-green-100 text-green-800';
            case 'UPDATE': return 'bg-blue-100 text-blue-800';
            case 'DELETE': return 'bg-red-100 text-red-800';
            case 'LOGIN': return 'bg-purple-100 text-purple-800';
            case 'LOGOUT': return 'bg-gray-100 text-gray-800';
            default: return 'bg-gray-100 text-gray-800';
        }
    };

    const formatTimestamp = (timestamp: string | undefined) => {
        if (!timestamp) return 'N/A';
        return new Date(timestamp).toLocaleString();
    };

    const formatData = (data: string | null | undefined) => {
        if (!data) return 'N/A';
        try {
            const parsed = JSON.parse(data);
            return JSON.stringify(parsed, null, 2);
        } catch {
            return data;
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold text-gray-900 mb-6">Audit Logs</h1>

            {/* Filters */}
            <div className="bg-white rounded-lg shadow p-6 mb-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Filters</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">User ID</label>
                        <input
                            type="text"
                            value={filters.userId}
                            onChange={(e) => handleFilterChange('userId', e.target.value)}
                            placeholder="Filter by user ID"
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Event Type</label>
                        <select
                            value={filters.eventType}
                            onChange={(e) => handleFilterChange('eventType', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="">All Events</option>
                            <option value="CREATE">Create</option>
                            <option value="UPDATE">Update</option>
                            <option value="DELETE">Delete</option>
                            <option value="LOGIN">Login</option>
                            <option value="LOGOUT">Logout</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                        <input
                            type="date"
                            value={filters.startDate}
                            onChange={(e) => handleFilterChange('startDate', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                        <input
                            type="date"
                            value={filters.endDate}
                            onChange={(e) => handleFilterChange('endDate', e.target.value)}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                </div>
                <div className="mt-4">
                    <button
                        onClick={clearFilters}
                        className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors"
                    >
                        Clear Filters
                    </button>
                </div>
            </div>

            {/* Logs Table */}
            <div className="bg-white rounded-lg shadow overflow-hidden">
                {loading ? (
                    <div className="flex items-center justify-center py-12">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : logs.length === 0 ? (
                    <div className="text-center py-12">
                        <p className="text-gray-500">No audit logs found.</p>
                    </div>
                ) : (
                    <>
                        <div className="overflow-x-auto">
                            <table className="min-w-full divide-y divide-gray-200">
                                <thead className="bg-gray-50">
                                    <tr>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Timestamp
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            User
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Event Type
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            IP Address
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Details
                                        </th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white divide-y divide-gray-200">
                                    {logs.map((log) => (
                                        <tr key={log.id} className="hover:bg-gray-50">
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                                {formatTimestamp(log.occurredAt)}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm font-medium text-gray-900">{log.userName || 'N/A'}</div>
                                                <div className="text-sm text-gray-500">{log.userId || 'N/A'}</div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getActionBadgeColor(log.eventType)}`}>
                                                    {log.eventType || 'N/A'}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                                {log.ipAddress || 'N/A'}
                                            </td>
                                            <td className="px-6 py-4 text-sm text-gray-500">
                                                <details className="cursor-pointer">
                                                    <summary className="text-blue-600 hover:text-blue-800">View Data</summary>
                                                    <pre className="mt-2 text-xs bg-gray-50 p-2 rounded overflow-x-auto">
                                                        {formatData(log.data)}
                                                    </pre>
                                                    {log.correlationId && (
                                                        <div className="mt-2 text-xs text-gray-600">
                                                            Correlation ID: {log.correlationId}
                                                        </div>
                                                    )}
                                                </details>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination */}
                        <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
                            <div className="flex-1 flex justify-between sm:hidden">
                                <button
                                    onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                                    disabled={currentPage === 1}
                                    className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
                                >
                                    Previous
                                </button>
                                <button
                                    onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                                    disabled={currentPage === totalPages}
                                    className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
                                >
                                    Next
                                </button>
                            </div>
                            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                                <div>
                                    <p className="text-sm text-gray-700">
                                        Page <span className="font-medium">{currentPage}</span> of{' '}
                                        <span className="font-medium">{totalPages}</span>
                                    </p>
                                </div>
                                <div>
                                    <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                                        <button
                                            onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                                            disabled={currentPage === 1}
                                            className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
                                        >
                                            Previous
                                        </button>
                                        <button
                                            onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                                            disabled={currentPage === totalPages}
                                            className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
                                        >
                                            Next
                                        </button>
                                    </nav>
                                </div>
                            </div>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};

export default AuditLogsPage;
