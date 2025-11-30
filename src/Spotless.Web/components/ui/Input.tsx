import React from "react";
import { cn } from "../../lib/utils";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  icon?: React.ReactNode;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className, label, error, icon, ...props }, ref) => {
    return (
      <div className="space-y-2">
        {label && (
          <label className="block text-sm font-semibold text-slate-700 dark:text-slate-200">
            {label}
          </label>
        )}
        <div className="relative">
          {icon && (
            <div className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400">
              {icon}
            </div>
          )}
          <input
            ref={ref}
            className={cn(
              "flex h-12 w-full rounded-xl border-2 border-slate-100 bg-slate-50/50 px-4 py-3 text-sm font-medium text-slate-900 placeholder:text-slate-400 transition-all duration-200",
              // Light mode focus/hover/background
              "focus:outline-none focus:ring-4 focus:ring-cyan-500/10 focus:border-cyan-500 focus:bg-white",
              "hover:border-cyan-200 hover:bg-white",
              // Disabled
              "disabled:cursor-not-allowed disabled:opacity-50 disabled:bg-slate-50",
              // Dark mode counterparts to avoid white-on-white issues
              "dark:border-slate-800 dark:bg-slate-900/50 dark:text-slate-50 dark:placeholder:text-slate-500",
              "dark:focus:border-cyan-500 dark:focus:bg-slate-900/50 dark:hover:border-slate-700 dark:hover:bg-slate-800/50",
              // Error state
              error &&
                "border-red-400 focus:border-red-500 focus:ring-red-500/10 bg-red-50/50",
              // If icon present add padding
              icon && "pl-10",
              className
            )}
            {...props}
          />
        </div>
        {error && (
          <p className="text-sm font-medium text-red-500 flex items-center gap-1">
            <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path
                fillRule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
                clipRule="evenodd"
              />
            </svg>
            {error}
          </p>
        )}
      </div>
    );
  }
);
Input.displayName = "Input";
