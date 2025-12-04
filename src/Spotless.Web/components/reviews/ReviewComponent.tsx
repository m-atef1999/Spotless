import React, { useState } from 'react';
import { Star, Send } from 'lucide-react';
import { Button } from '../ui/Button';
import { ReviewsService } from '../../lib/api';
import { useToast } from '../ui/Toast';

interface ReviewComponentProps {
    orderId: string;
    onReviewSubmitted: (rating: number) => void;
}

export const ReviewComponent: React.FC<ReviewComponentProps> = ({ orderId, onReviewSubmitted }) => {
    const [rating, setRating] = useState(0);
    const [hoveredRating, setHoveredRating] = useState(0);
    const [comment, setComment] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { addToast } = useToast();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (rating === 0) return;

        setIsSubmitting(true);
        try {
            await ReviewsService.postApiReviews({
                requestBody: {
                    orderId,
                    rating,
                    comment
                }
            });
            addToast('Review submitted successfully!', 'success');
            onReviewSubmitted(rating);
        } catch (error) {
            console.error('Failed to submit review:', error);
            addToast('Failed to submit review. Please try again.', 'error');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 mt-6">
            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                <Star className="w-5 h-5 text-cyan-500" />
                Rate your experience
            </h3>

            <form onSubmit={handleSubmit} className="space-y-6">
                <div className="space-y-2">
                    <div className="flex gap-2 justify-center">
                        {[1, 2, 3, 4, 5].map((star) => (
                            <button
                                key={star}
                                type="button"
                                onClick={() => setRating(star)}
                                onMouseEnter={() => setHoveredRating(star)}
                                onMouseLeave={() => setHoveredRating(0)}
                                className="focus:outline-none transition-transform hover:scale-110 p-1"
                            >
                                <Star
                                    className={`w-8 h-8 ${star <= (hoveredRating || rating)
                                        ? 'fill-yellow-400 text-yellow-400'
                                        : 'text-slate-300 dark:text-slate-600'
                                        }`}
                                />
                            </button>
                        ))}
                    </div>
                    <p className="text-center text-sm text-slate-500 font-medium h-5">
                        {rating === 1 && "Terrible"}
                        {rating === 2 && "Bad"}
                        {rating === 3 && "Okay"}
                        {rating === 4 && "Good"}
                        {rating === 5 && "Excellent!"}
                    </p>
                </div>

                <div className="space-y-2">
                    <textarea
                        rows={3}
                        value={comment}
                        onChange={(e) => setComment(e.target.value)}
                        className="w-full px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500 focus:border-transparent transition-all resize-none text-sm"
                        placeholder="Share your feedback (optional)..."
                    />
                </div>

                <Button
                    type="submit"
                    className="w-full bg-cyan-500 hover:bg-cyan-600 text-white"
                    isLoading={isSubmitting}
                    disabled={rating === 0}
                >
                    <Send className="w-4 h-4 mr-2" />
                    Submit Review
                </Button>
            </form>
        </div>
    );
};
