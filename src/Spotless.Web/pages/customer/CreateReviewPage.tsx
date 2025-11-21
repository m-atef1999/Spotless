import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Star, Send, ArrowLeft } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { ReviewsService } from '../../lib/api';

export const CreateReviewPage: React.FC = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const orderId = searchParams.get('orderId');

    const [rating, setRating] = useState(0);
    const [comment, setComment] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [hoveredRating, setHoveredRating] = useState(0);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!orderId) {
            alert('No order ID provided');
            return;
        }
        if (rating === 0) {
            alert('Please select a rating');
            return;
        }

        setIsSubmitting(true);
        try {
            await ReviewsService.postApiReviews({
                requestBody: {
                    orderId,
                    rating,
                    comment
                }
            });
            alert('Review submitted successfully!');
            navigate('/customer/orders');
        } catch (error) {
            console.error('Failed to submit review:', error);
            alert('Failed to submit review. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    if (!orderId) {
        return (
            <DashboardLayout role="Customer">
                <div className="text-center py-12">
                    <p className="text-red-500">Error: No order specified for review.</p>
                    <Button variant="ghost" onClick={() => navigate('/customer/orders')} className="mt-4">
                        Back to Orders
                    </Button>
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-2xl mx-auto space-y-8">
                <div className="flex items-center gap-4">
                    <Button variant="ghost" size="sm" onClick={() => navigate(-1)}>
                        <ArrowLeft className="w-4 h-4 mr-2" />
                        Back
                    </Button>
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                        Write a Review
                    </h1>
                </div>

                <div className="bg-white dark:bg-slate-900 p-8 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
                    <form onSubmit={handleSubmit} className="space-y-8">
                        <div className="space-y-4">
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                                How would you rate your experience?
                            </label>
                            <div className="flex gap-2">
                                {[1, 2, 3, 4, 5].map((star) => (
                                    <button
                                        key={star}
                                        type="button"
                                        onClick={() => setRating(star)}
                                        onMouseEnter={() => setHoveredRating(star)}
                                        onMouseLeave={() => setHoveredRating(0)}
                                        className="focus:outline-none transition-transform hover:scale-110"
                                    >
                                        <Star
                                            className={`w-10 h-10 ${star <= (hoveredRating || rating)
                                                    ? 'fill-yellow-400 text-yellow-400'
                                                    : 'text-slate-300 dark:text-slate-600'
                                                }`}
                                        />
                                    </button>
                                ))}
                            </div>
                            <p className="text-sm text-slate-500">
                                {rating === 1 && "Terrible"}
                                {rating === 2 && "Bad"}
                                {rating === 3 && "Okay"}
                                {rating === 4 && "Good"}
                                {rating === 5 && "Excellent!"}
                            </p>
                        </div>

                        <div className="space-y-4">
                            <label htmlFor="comment" className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                                Share your feedback (optional)
                            </label>
                            <textarea
                                id="comment"
                                rows={4}
                                value={comment}
                                onChange={(e) => setComment(e.target.value)}
                                className="w-full px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all resize-none"
                                placeholder="Tell us what you liked or how we can improve..."
                            />
                        </div>

                        <Button
                            type="submit"
                            className="w-full bg-cyan-500 hover:bg-cyan-600 text-white py-6 text-lg"
                            isLoading={isSubmitting}
                            disabled={rating === 0}
                        >
                            <Send className="w-5 h-5 mr-2" />
                            Submit Review
                        </Button>
                    </form>
                </div>
            </div>
        </DashboardLayout>
    );
};
