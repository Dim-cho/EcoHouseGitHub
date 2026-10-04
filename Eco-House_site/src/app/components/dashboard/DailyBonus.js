"use client";

import { useActionState } from "react";
import { claimDailyCoinsAction } from "@/app/actions/rewards";

// 7-day streak. Claimed coins reach the game on its next sync.
export default function DailyBonus({ streak, claimedToday }) {
  const [state, action, pending] = useActionState(claimDailyCoinsAction, null);

  const claimed = claimedToday || Boolean(state?.amount);
  const currentStreak = state?.streak ?? streak;

  return (
    <div className="flex h-full flex-col rounded-xl border border-line bg-paper p-6">
      <p className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">Daily bonus</p>
      <p className="mt-2 text-xl font-semibold tracking-tight text-ink">
        {currentStreak}-day streak
      </p>

      {/* Capped: on a full-width phone card, seven aspect-square cells become
          huge boxes. */}
      <ol
        className="mt-5 grid max-w-[280px] grid-cols-7 gap-1.5"
        aria-label={`Day ${currentStreak} of 7`}
      >
        {Array.from({ length: 7 }, (_, i) => (
          <li
            key={i}
            className={`flex aspect-square items-center justify-center rounded-md border font-pixel text-xs ${
              i < currentStreak ? "border-brand bg-brand text-paper" : "border-line text-ink-soft"
            }`}
          >
            {i + 1}
          </li>
        ))}
      </ol>

      {state?.error && <p className="mt-4 text-sm text-ink-soft">{state.error}</p>}

      <div className="mt-auto pt-6">
        {claimed ? (
          <p className="rounded-lg border border-line px-4 py-3 text-center text-sm text-ink-soft">
            {state?.amount ? (
              <>
                <span className="font-medium text-brand">
                  +{state.amount.toLocaleString("en-GB")} coins
                </span>{" "}
                claimed · back tomorrow
              </>
            ) : (
              "Claimed today · back tomorrow"
            )}
          </p>
        ) : (
          <form action={action}>
            <button
              type="submit"
              disabled={pending}
              className="w-full rounded-lg bg-ink px-5 py-3 font-medium text-paper transition-colors hover:bg-ink/85 disabled:opacity-60"
            >
              {pending ? "Claiming…" : "Claim today's coins"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
