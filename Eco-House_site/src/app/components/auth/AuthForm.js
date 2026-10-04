"use client";

import Link from "next/link";
import { useActionState } from "react";
import AuthField from "./AuthField";

export default function AuthForm({ mode, action }) {
  const [state, formAction, pending] = useActionState(action, null);
  const isSignUp = mode === "signup";

  return (
    <>
      <p className="font-mono text-[13px] uppercase tracking-[.18em] text-ink-soft">
        {isSignUp ? "Join the board" : "Welcome back"}
      </p>
      <h1 className="mt-2 text-[28px] font-semibold leading-tight tracking-[-.03em] text-ink">
        {isSignUp ? "Create an account" : "Log in"}
      </h1>
      <p className="mt-1.5 text-[15px] text-ink-soft">
        {isSignUp
          ? "One account for the game and the site. Your progress follows it."
          : "Use the same username and password as in the game."}
      </p>

      {state?.error && (
        <p
          role="alert"
          className="mt-6 rounded-lg border border-[#e7c3b0] bg-[#fbefe8] px-4 py-3 text-sm text-[#7a3316]"
        >
          {state.error}
        </p>
      )}

      <form action={formAction} className="mt-6 space-y-4">
        <AuthField
          label="Username"
          name="username"
          autoComplete="username"
          hint={isSignUp ? "3–20 characters: letters, numbers or underscores." : undefined}
        />
        <AuthField
          label="Password"
          name="password"
          type="password"
          autoComplete={isSignUp ? "new-password" : "current-password"}
          hint={isSignUp ? "At least 8 characters." : undefined}
        />
        {isSignUp && (
          <AuthField
            label="Confirm password"
            name="confirm"
            type="password"
            autoComplete="new-password"
          />
        )}

        <button
          type="submit"
          disabled={pending}
          className="w-full rounded-lg bg-ink px-6 py-3 font-medium text-paper transition-colors hover:bg-ink/85 disabled:opacity-60"
        >
          {pending ? "Just a moment…" : isSignUp ? "Create account" : "Log in"}
        </button>
      </form>

      <p className="mt-6 border-t border-line pt-4 text-sm text-ink-soft">
        {isSignUp ? "Already have one? " : "New here? "}
        <Link
          href={isSignUp ? "/login" : "/signup"}
          className="font-medium text-ink underline decoration-line underline-offset-4 transition-colors hover:decoration-ink"
        >
          {isSignUp ? "Log in" : "Create an account"}
        </Link>
      </p>
    </>
  );
}
