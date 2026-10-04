"use client";

import { useActionState } from "react";
import { searchPlayerAction } from "@/app/actions/leaderboard";

const fmt = (n) => n.toLocaleString("en-US");

// Finds anyone on the board, not just the visible top 10.
export default function PlayerSearch() {
  const [result, action, pending] = useActionState(searchPlayerAction, null);

  return (
    <div className="mt-10 border-t border-line pt-8">
      <form action={action} className="flex flex-wrap items-end gap-3">
        <div className="min-w-[220px] flex-1">
          <label
            htmlFor="lb-search"
            className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft"
          >
            Find a player
          </label>
          <input
            id="lb-search"
            name="username"
            type="text"
            placeholder="Username"
            autoComplete="off"
            className="mt-2 block w-full rounded-lg border border-line bg-paper px-4 py-2.5 text-ink outline-none transition placeholder:text-ink-soft/60 focus:border-ink focus:ring-2 focus:ring-brand/25"
          />
        </div>
        <button
          type="submit"
          disabled={pending}
          className="rounded-lg bg-ink px-5 py-2.5 font-medium text-paper transition-colors hover:bg-ink/85 disabled:opacity-60"
        >
          {pending ? "Searching…" : "Search"}
        </button>
      </form>

      {result?.status === "ok" && (
        <div className="mt-6 flex flex-wrap items-center justify-between gap-4 rounded-xl border border-ink/80 bg-paper px-5 py-4">
          <div className="flex items-center gap-5">
            <span className="font-pixel text-[28px] leading-none text-brand">
              #{result.rank}
            </span>
            <span className="font-medium text-ink">{result.username}</span>
          </div>
          <div className="flex items-center gap-6 font-mono text-sm tabular-nums">
            <span className="text-ink-soft">{fmt(result.co2Kg)} kg CO₂</span>
            <span className="text-ink">{fmt(result.kwhSaved)} kWh</span>
          </div>
        </div>
      )}

      {result?.status === "unranked" && (
        <p className="mt-6 rounded-xl border border-line bg-paper px-5 py-4 text-ink-soft">
          <span className="font-medium text-ink">{result.username}</span> has an account but
          hasn&apos;t saved any energy yet, so they&apos;re not on the board.
        </p>
      )}

      {result?.status === "not-found" && (
        <p className="mt-6 rounded-xl border border-line bg-paper px-5 py-4 text-ink-soft">
          No player by that name.
        </p>
      )}
    </div>
  );
}
