import React, { useState, useEffect } from 'react';
import { Search, Star, MessageSquare, User, Trash2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { ReviewsService, type ReviewDto, type PagedResponse } from '../../lib/api';

export const ReviewsManagementPage: React.FC = () => {
    const [reviews, setReviews] = useState<ReviewDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const fetchReviews = async () => {
            try {
                const response: PagedResponse = await ReviewsService.getApiReviewsAdminAll({
                    pageNumber: 1,
                    pageSize: 100
                });
                if (response.data) {
                    setReviews(response.data as ReviewDto[]);
                }
            } catch (error) {
                console.error('Failed to fetch reviews', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchReviews();
    }, []);

    const filteredReviews = reviews.filter(review =>
        review.comment?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        review.customerId?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleDeleteReview = (_id: string) => {

        alert('Delete Review functionality is currently mocked (API endpoint missing).');
    };

    const renderStars = (rating: number | undefined) => {
        const stars = [];
        const r = rating || 0;
        for (let i = 1; i <= 5; i++) {
            stars.push(
                <Star
                    key={i}
                    className={`w-4 h-4 ${i <= r ? 'text-yellow-400 fill-current' : 'text-slate-300'}`}
                />
            );
        }
        return <div className="flex gap-0.5">{stars}</div>;
    };

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Reviews Management
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Monitor and moderate customer feedback.
                        </p>
                    </div>
                    <div className="w-full sm:w-72">
                        <Input
                            placeholder="Search reviews..."
                            icon={<Search className="w-5 h-5" />}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="w-full text-left border-collapse">
                            <thead>
                                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Customer</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Rating</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Comment</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Order ID</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider text-right">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                                {isLoading ? (
                                    <tr>
                                        <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                                            Loading reviews...
                                        </td>
                                    </tr>
                                ) : filteredReviews.length === 0 ? (
                                    <tr>
                                        <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                                            No reviews found.
                                        </td>
                                    </tr>
                                ) : (
                                    filteredReviews.map((review) => (
                                        <tr key={review.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2">
                                                    <User className="w-4 h-4 text-slate-400" />
                                                    <span className="font-medium text-slate-900 dark:text-white truncate max-w-[150px]">
                                                        {review.customerId}
                                                    </span>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                {renderStars(review.rating)}
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-start gap-2">
                                                    <MessageSquare className="w-4 h-4 text-slate-400 mt-0.5 shrink-0" />
                                                    <p className="text-sm text-slate-600 dark:text-slate-300 line-clamp-2">
                                                        {review.comment}
                                                    </p>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-500 font-mono">
                                                {review.orderId}
                                            </td>
                                            <td className="px-6 py-4 text-right">
                                                <Button
                                                    size="sm"
                                                    variant="outline"
                                                    className="text-red-600 hover:bg-red-50 border-red-200"
                                                    onClick={() => handleDeleteReview(review.id!)}
                                                >
                                                    <Trash2 className="w-4 h-4" />
                                                </Button>
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
