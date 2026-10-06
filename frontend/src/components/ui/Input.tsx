import { InputHTMLAttributes, forwardRef, ReactNode } from "react";

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  rightElement?: ReactNode;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ className = "", label, error, rightElement, ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="mb-1.5 block text-sm font-medium text-ink/80" htmlFor={props.id}>
            {label}
          </label>
        )}
        <div className="relative">
          <input
            ref={ref}
            className={`flex h-10 w-full rounded-md border bg-white px-3 py-2 text-sm text-ink transition-colors file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-ink/40 focus:outline-none focus:ring-2 focus:ring-brand focus:ring-offset-1 disabled:cursor-not-allowed disabled:opacity-50 ${
              error ? "border-danger focus:ring-danger" : "border-line focus:border-brand"
            } ${rightElement ? "pr-10" : ""} ${className}`}
            {...props}
          />
          {rightElement && (
            <div className="absolute inset-y-0 right-0 flex items-center pr-3">
              {rightElement}
            </div>
          )}
        </div>
        {error && <p className="mt-1.5 text-xs text-danger">{error}</p>}
      </div>
    );
  }
);
Input.displayName = "Input";

