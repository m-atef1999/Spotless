import { useState, useEffect } from 'react';
import { CustomersService, type CustomerDto, type PagedResponse } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';

export function CustomerManagementPage() {
    const [customers, setCustomers] = useState<CustomerDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [emailFilter, setEmailFilter] = useState('');
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const { addToast } = useToast();

    useEffect(() => {
        fetchCustomers();
    }, [pageNumber, searchTerm, emailFilter]);

    const fetchCustomers = async () => {
        try {
            setLoading(true);
            const response: PagedResponse = await CustomersService.getApiCustomers({
                nameFilter: searchTerm || undefined,
                emailFilter: emailFilter || undefined,
                pageNumber,
                pageSize: 25
            });
            setCustomers((response.data as CustomerDto[]) || []);
            setTotalPages(response.totalPages || 1);
        } catch (error) {
            addToast('Failed to load customers', 'error');
            console.error('Error fetching customers:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = () => {
        setPageNumber(1);
        fetchCustomers();
    };

    const handleClearFilters = () => {
        setSearchTerm('');
        setEmailFilter('');
        setPageNumber(1);
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-7xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900">Customer Management</h1>
                    <p className="text-gray-600 mt-2">Manage and view all customer accounts</p>
                </div>

                {/* Filters */}
                <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Search by Name
                            </label>
                            <input
                                type="text"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                placeholder="Enter customer name..."
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Search by Email
                            </label>
                            <input
                                type="email"
                                value={emailFilter}
                                onChange={(e) => setEmailFilter(e.target.value)}
                                placeholder="Enter email..."
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                            />
                        </div>
                        <div className="flex items-end gap-2">
                            <button
                                onClick={handleSearch}
                                className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                            >
                                Search
                            </button>
                            <button
                                onClick={handleClearFilters}
                                className="flex-1 bg-gray-200 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-300 transition-colors"
                            >
                                Clear
                            </button>
                        </div>
                    </div>
                </div>

                {/* Customer Table */}
                <div className="bg-white rounded-lg shadow-sm overflow-hidden">
                    {loading ? (
                        <div className="flex items-center justify-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                        </div>
                    ) : customers.length === 0 ? (
                        <div className="text-center py-12">
                            <p className="text-gray-500 text-lg">No customers found</p>
                        </div>
                    ) : (
                        <>
                            <table className="min-w-full divide-y divide-gray-200">
                                <thead className="bg-gray-50">
                                    <tr>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Name
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Email
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Phone
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Type
                                        </th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Wallet Balance
                                        </th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white divide-y divide-gray-200">
                                    {customers.map((customer) => (
                                        <tr key={customer.id} className="hover:bg-gray-50 cursor-pointer">
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm font-medium text-gray-900">{customer.name}</div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm text-gray-500">{customer.email}</div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm text-gray-500">{customer.phone || 'N/A'}</div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 text-blue-800">
                                                    {customer.type}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="text-sm text-gray-900">
                                                    {customer.walletBalance?.toFixed(2)} EGP
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>

                            {/* Pagination */}
                            {totalPages > 1 && (
                                <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
                                    <div className="flex-1 flex justify-between sm:hidden">
                                        <button
                                            onClick={() => setPageNumber(Math.max(1, pageNumber - 1))}
                                            disabled={pageNumber === 1}
                                            className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
                                        >
                                            Previous
                                        </button>
                                        <button
                                            onClick={() => setPageNumber(Math.min(totalPages, pageNumber + 1))}
                                            disabled={pageNumber === totalPages}
                                            className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
                                        >
                                            Next
                                        </button>
                                    </div>
                                    <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                                        <div>
                                            <p className="text-sm text-gray-700">
                                                Page <span className="font-medium">{pageNumber}</span> of{' '}
                                                <span className="font-medium">{totalPages}</span>
                                            </p>
                                        </div>
                                        <div>
                                            <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                                                <button
                                                    onClick={() => setPageNumber(Math.max(1, pageNumber - 1))}
                                                    disabled={pageNumber === 1}
                                                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                                                >
                                                    Previous
                                                </button>
                                                <button
                                                    onClick={() => setPageNumber(Math.min(totalPages, pageNumber + 1))}
                                                    disabled={pageNumber === totalPages}
                                                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                                                >
                                                    Next
                                                </button>
                                            </nav>
                                        </div>
                                    </div>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}

