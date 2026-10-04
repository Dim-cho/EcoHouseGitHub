"use client";

import Link from "next/link";
import { useActionState } from "react";
import { claimEcoPointsAction } from "@/app/actions/claims";

// End-of-post reward: once per post per account. Points reach the game on its
// next sync.
export default function ClaimEcoPoints({ slug, points, signedIn, alreadyClaimed }) {
  const [state, action, pending] = useActionState(claimEcoPointsAction, null);

  const done = alreadyClaimed || Boolean(state?.points);

  return (
    <section
      aria-labelledby="claim-title"
      className="relative overflow-hidden rounded-xl border border-ink/80 bg-paper p-6 sm:p-8"
    >
      <div className="grid gap-6 sm:grid-cols-[auto_1fr_auto] sm:items-center">
        <span
          className={`flex h-16 w-16 items-center justify-center rounded-lg font-pixel text-xl ${
            done ? "bg-brand text-paper" : "bg-band text-brand"
          }`}
        >
          +{points}
        </span>

        <div>
          <p className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
            Eco points
          </p>
          <h2 id="claim-title" className="mt-1 text-xl font-semibold tracking-tight text-ink">
            {done ? "Claimed. Nice reading." : "You made it to the end."}
          </h2>
          <p className="mt-1 text-base text-ink-soft">
            {done
              ? "They'll be in the game the next time it syncs."
              : signedIn
                ? `Claim ${points} eco points for reading this post. Once per post.`
                : "Log in to claim eco points for reading. They go straight into the game."}
          </p>
          {state?.error && !done && (
            <p role="alert" className="mt-2 text-[13px] text-[#a3461f]">
              {state.error}
            </p>
          )}
        </div>

        {done ? (
          <span className="justify-self-start rounded-lg border border-line px-5 py-3 font-medium text-ink-soft sm:justify-self-end">
            Claimed ✓
          </span>
        ) : signedIn ? (
          <form action={action} className="justify-self-start sm:justify-self-end">
            <input type="hidden" name="slug" value={slug} />
            <button
              type="submit"
              disabled={pending}
              className="rounded-lg bg-ink px-5 py-3 font-medium text-paper transition-colors hover:bg-ink/85 disabled:opacity-60"
            >
              {pending ? "Claiming…" : `Claim +${points}`}
            </button>
          </form>
        ) : (
          <Link
            href="/login"
            className="justify-self-start rounded-lg bg-ink px-5 py-3 font-medium text-paper transition-colors hover:bg-ink/85 sm:justify-self-end"
          >
            Log in to claim
          </Link>
        )}
      </div>
    </section>
  );
}
