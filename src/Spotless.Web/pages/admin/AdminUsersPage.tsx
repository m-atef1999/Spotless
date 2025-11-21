import React, { useState, useEffect } from 'react';
import { Search, Shield, UserPlus, Mail, Calendar } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { AdminsService, type AdminDto } from '../../lib/api';

export const AdminUsersPage: React.FC = () => {
    const [admins, setAdmins] = useState<AdminDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const fetchAdmins = async () => {
            try {
                const response = await AdminsService.getApiAdmins({
                    searchTerm: searchTerm || undefined,
                    pageNumber: 1,
                    pageSize: 50
                });
                setAdmins(response.data || []);
            } catch (error) {
                console.error('Failed to fetch admins', error);
            } finally {
                setIsLoading(false);
            }
        };

        const debounce = setTimeout(fetchAdmins, 300);
        return () => clearTimeout(debounce);
    }, [searchTerm]);

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            System Administrators
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Manage users with administrative access.
                        </p>
                    </div>
                    <div className="flex gap-4">
                        <div className="w-full sm:w-72">
                            <Input
                                placeholder="Search admins..."
                                icon={<Search className="w-5 h-5" />}
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <Button>
                            <UserPlus className="w-4 h-4 mr-2" />
                            Add Admin
                        </Button>
                    </div>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="w-full text-left border-collapse">
                            <thead>
                                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Admin User</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Email</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Role</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Joined</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                                {isLoading ? (
                                    <tr>
                                        <td colSpan={4} className="px-6 py-8 text-center text-slate-500">
                                            Loading administrators...
                                        </td>
                                    </tr>
                                ) : admins.length === 0 ? (
                                    <tr>
                                        <td colSpan={4} className="px-6 py-8 text-center text-slate-500">
                                            No administrators found.
                                        </td>
                                    </tr>
                                ) : (
                                    admins.map((admin) => (
                                        <tr key={admin.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-3">
                                                    <div className="w-10 h-10 bg-purple-100 dark:bg-purple-900/30 rounded-full flex items-center justify-center text-purple-600 dark:text-purple-400">
                                                        <Shield className="w-5 h-5" />
                                                    </div>
                                                    <div>
                                                        <div className="font-medium text-slate-900 dark:text-white">
                                                            {admin.name}
                                                        </div>
                                                        <div className="text-xs text-slate-500">ID: {admin.id?.substring(0, 8)}...</div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2 text-slate-600 dark:text-slate-300">
                                                    <Mail className="w-4 h-4 text-slate-400" />
                                                    {admin.email}
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
                                                    Super Admin
                                                </span>
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2 text-slate-500">
                                                    <Calendar className="w-4 h-4" />
                                                    {new Date().toLocaleDateString()} {/* Placeholder as CreatedAt might be missing in DTO */}
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
