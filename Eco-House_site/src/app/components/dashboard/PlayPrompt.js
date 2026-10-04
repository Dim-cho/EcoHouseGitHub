import Link from "next/link";

const STEPS = [
  ["Get the game", "Free while in development. Builds are coming soon."],
  ["Log in with this account", "Same username and password, no extra setup."],
  [
    "Play, and it shows up here",
    "Coins, eco points, energy saved and your rank sync automatically.",
  ],
];

// Shown when the account has never synced from the game.
export default function PlayPrompt() {
  return (
    <div className="grid gap-8 rounded-xl border border-ink/80 bg-paper p-6 sm:p-8 lg:grid-cols-[1fr_1.3fr] lg:items-center">
      <div>
        <p className="font-pixel text-sm uppercase tracking-wider text-brand">No save found</p>
        <p className="mt-3 text-2xl font-semibold leading-snug tracking-tight text-ink">
          You haven&apos;t started the game yet.
        </p>
        <p className="mt-2 leading-relaxed text-ink-soft">
          Your stats will live here once you play. Until then, eco points from the blog are already
          counting.
        </p>
        <Link
          href="/#download"
          className="mt-6 inline-flex items-center gap-2 rounded-lg bg-ink px-5 py-3 font-medium text-paper transition-colors hover:bg-ink/85"
        >
          Try the game
          <span className="rounded bg-paper/15 px-1.5 py-0.5 font-mono text-[12px] uppercase tracking-wider">
            Soon
          </span>
        </Link>
      </div>

      <ol className="border-b border-line">
        {STEPS.map(([title, detail], i) => (
          <li key={title} className="grid grid-cols-[2.25rem_1fr] gap-x-3 border-t border-line py-4">
            <span className="font-pixel text-lg text-ink-soft">0{i + 1}</span>
            <span>
              <span className="block font-medium text-ink">{title}</span>
              <span className="block text-sm text-ink-soft">{detail}</span>
            </span>
          </li>
        ))}
      </ol>
    </div>
  );
}
