"use client";

import { useTransition } from "react";
import { useRouter } from "next/navigation";

export default function LeaderboardError() {
  const router = useRouter();
  const [pending, start] = useTransition();

  return (
    <div className="border-y border-line py-14 text-center" role="alert">
      <p className="font-pixel text-sm uppercase tracking-wider text-[#a3461f]">Connection lost</p>
      <p className="mt-3 text-xl font-semibold tracking-tight text-ink">
        We can&apos;t reach the leaderboard right now.
      </p>
      <p className="mx-auto mt-2 max-w-md text-ink-soft">
        Your progress is safe in the game. Scores will be back as soon as the connection is.
      </p>
      <button
        type="button"
        onClick={() => start(() => router.refresh())}
        disabled={pending}
        className="mt-6 rounded-lg border border-line px-5 py-3 font-medium text-ink transition-colors hover:border-ink disabled:opacity-60"
      >
        {pending ? "Trying…" : "Try again"}
      </button>
    </div>
  );
}
