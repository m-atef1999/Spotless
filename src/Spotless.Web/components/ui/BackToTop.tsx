import React, { useState, useEffect } from 'react';
import { ArrowUp } from 'lucide-react';
import { Button } from './Button';

export const BackToTop: React.FC = () => {
    const [isVisible, setIsVisible] = useState(false);

    const toggleVisibility = () => {
        if (window.pageYOffset > 300) {
            setIsVisible(true);
        } else {
            setIsVisible(false);
        }
    };

    const scrollToTop = () => {
        window.scrollTo({
            top: 0,
            behavior: 'smooth',
        });
    };

    useEffect(() => {
        window.addEventListener('scroll', toggleVisibility);
        return () => {
            window.removeEventListener('scroll', toggleVisibility);
        };
    }, []);

    return (
        <>
            {isVisible && (
                <div className="fixed bottom-24 right-4 z-[100] animate-fade-in">
                    <Button
                        onClick={scrollToTop}
                        className="rounded-full shadow-lg bg-cyan-500 hover:bg-cyan-600 text-white w-12 h-12 flex items-center justify-center p-0"
                        aria-label="Back to top"
                    >
                        <ArrowUp className="w-6 h-6" />
                    </Button>
                </div>
            )}
        </>
    );
};
