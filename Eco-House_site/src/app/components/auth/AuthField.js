"use client";

import { useId, useState } from "react";

export default function AuthField({
  label,
  name,
  type = "text",
  autoComplete,
  hint,
  required = true,
}) {
  const id = useId();
  const [show, setShow] = useState(false);
  const isPassword = type === "password";

  return (
    <div>
      <div className="flex items-baseline justify-between">
        <label htmlFor={id} className="text-sm font-medium text-ink">
          {label}
        </label>
        {isPassword && (
          <button
            type="button"
            onClick={() => setShow((s) => !s)}
            className="font-mono text-[13px] uppercase tracking-wider text-ink-soft transition-colors hover:text-ink"
          >
            {show ? "Hide" : "Show"}
          </button>
        )}
      </div>

      <input
        id={id}
        name={name}
        type={isPassword && show ? "text" : type}
        autoComplete={autoComplete}
        required={required}
        aria-describedby={hint ? `${id}-msg` : undefined}
        className="mt-1.5 block w-full rounded-lg border border-line bg-paper px-4 py-2.5 text-ink outline-none transition placeholder:text-ink-soft/60 focus:border-ink focus:ring-2 focus:ring-brand/25"
      />

      {hint && (
        <p id={`${id}-msg`} className="mt-1.5 text-[13px] text-ink-soft">
          {hint}
        </p>
      )}
    </div>
  );
}
