import Link from "next/link";
import { getSession } from "@/lib/session";
import { getPlayerStats } from "@/lib/stats";

// `floating` lets it sit over the hero's sky on the landing page. Everywhere
// else it's a normal bar in the flow, or it would cover the content.
export default async function SiteNav({ floating = false }) {
  const session = await getSession();
  const stats = session ? await getPlayerStats(session.username) : null;

  return (
    <header
      className={
        floating ? "absolute inset-x-0 top-0 z-20" : "relative border-b border-line bg-paper"
      }
    >
      {/* Width and padding track the hero's container so the logo lines up
          with the headline. */}
      <div className="mx-auto flex h-20 max-w-[1320px] items-center justify-between px-5 sm:px-8">
        {/* The logo carries the wordmark, so no text beside it. Pixelated
            rendering keeps the pixel art crisp instead of smoothing it. */}
        <Link href="/" className="flex items-center">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src="/logo/ecohouse-logo.svg"
            alt="Eco-House"
            className="h-9 w-auto"
            style={{ imageRendering: "pixelated" }}
          />
        </Link>

        <nav
          className="hidden items-center gap-9 text-[15px] text-ink-soft md:flex"
          aria-label="Main"
        >
          <Link className="transition-colors hover:text-ink" href="/#learn">
            How it works
          </Link>
          <Link className="transition-colors hover:text-ink" href="/#leaderboard">
            Leaderboard
          </Link>
          <Link className="transition-colors hover:text-ink" href="/learn">
            Blog
          </Link>
          <Link className="transition-colors hover:text-ink" href="/#download">
            Download
          </Link>
        </nav>

        {session ? (
          <div className="flex items-center gap-4 text-[15px]">
            <span className="hidden items-center gap-2 rounded-full border border-line px-3 py-1.5 sm:inline-flex">
              <span className="font-pixel text-brand">{stats.ecoPoints}</span>
              <span className="font-mono text-[11px] uppercase tracking-wider text-ink-soft">
                Eco
              </span>
            </span>
            <Link
              href="/dashboard"
              className="flex items-center gap-2.5 font-medium text-ink transition-opacity hover:opacity-80"
            >
              <span className="flex h-8 w-8 items-center justify-center rounded-full bg-ink font-mono text-xs text-paper">
                {session.username.slice(0, 2).toUpperCase()}
              </span>
              <span className="hidden sm:inline">{session.username}</span>
            </Link>
          </div>
        ) : (
          <div className="flex items-center gap-5 text-[15px]">
            <Link
              href="/login"
              className="hidden text-ink-soft transition-colors hover:text-ink sm:inline"
            >
              Log in
            </Link>
            <Link
              href="/signup"
              className="rounded-lg bg-ink px-4 py-2.5 font-medium text-paper transition-colors hover:bg-ink/85"
            >
              Sign up
            </Link>
          </div>
        )}
      </div>
    </header>
  );
}
