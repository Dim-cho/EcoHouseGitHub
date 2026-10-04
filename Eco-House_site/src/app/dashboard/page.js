import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { getBonusState } from "@/lib/bonus";
import { getPlayerStats, getPlayerRank } from "@/lib/stats";
import { getTotalSaved } from "@/lib/leaderboard";
import { ARTICLES } from "@/lib/articles";
import { ECO_POINTS_PER_POST } from "@/lib/claims";
import { logOutAction } from "@/app/actions/auth";
import SponsorRow from "@/app/components/dashboard/SponsorRow";
import ProgressStats from "@/app/components/dashboard/ProgressStats";
import PlayPrompt from "@/app/components/dashboard/PlayPrompt";
import DailyBonus from "@/app/components/dashboard/DailyBonus";
import BlogBanner from "@/app/components/dashboard/BlogBanner";

export const dynamic = "force-dynamic";

export const metadata = { title: "Dashboard · Eco-House" };

export default async function DashboardPage() {
  const session = await getSession();
  if (!session) redirect("/login");

  const [bonus, stats, rank, totals] = await Promise.all([
    getBonusState(session.username),
    getPlayerStats(session.username),
    getPlayerRank(session.username),
    getTotalSaved(),
  ]);

  // No sync yet means there's nothing to show but an invitation to play.
  const hasPlayed = Boolean(stats.lastSync);

  return (
    <main className="mx-auto w-full max-w-[1320px] space-y-16 px-5 py-16 sm:px-8 sm:py-20">
      <header className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="font-mono text-[13px] uppercase tracking-[.18em] text-ink-soft">
            Dashboard
          </p>
          {/* Usernames can be 20 characters with no spaces, so the heading has
              to be allowed to break inside the word. */}
          <h1 className="mt-3 break-words text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]">
            Hi, {session.username}.
          </h1>
        </div>

        <form action={logOutAction}>
          <button
            type="submit"
            className="rounded-lg border border-line px-4 py-2.5 text-[15px] font-medium text-ink-soft transition-colors hover:border-ink-soft hover:text-ink"
          >
            Log out
          </button>
        </form>
      </header>

      <SponsorRow />

      <section aria-labelledby="prog-title">
        <h2 id="prog-title" className="text-2xl font-semibold tracking-tight text-ink">
          Your game
        </h2>
        <div className="mt-6 grid gap-6 lg:grid-cols-[1fr_320px]">
          {hasPlayed ? (
            <ProgressStats stats={stats} rank={rank} totalPlayers={totals.players} />
          ) : (
            <PlayPrompt />
          )}
          <DailyBonus streak={bonus.streak} claimedToday={bonus.claimedToday} />
        </div>
      </section>

      <BlogBanner guide={ARTICLES[1]} points={ECO_POINTS_PER_POST} />
    </main>
  );
}
