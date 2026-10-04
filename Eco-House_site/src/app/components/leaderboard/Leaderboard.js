import Kicker from "@/app/components/ui/Kicker";
import Accent from "@/app/components/ui/Accent";
import { getLeaderboard } from "@/lib/leaderboard";
import StatFigures from "./StatFigures";
import LeaderboardTable from "./LeaderboardTable";
import LeaderboardEmpty from "./LeaderboardEmpty";
import LeaderboardError from "./LeaderboardError";
import PlayerSearch from "./PlayerSearch";

const STATUS = {
  ok: { text: "Live · updated just now", dot: "bg-brand pulse-soft" },
  empty: { text: "Waiting for the first sync", dot: "bg-[#d9a12b]" },
  error: { text: "Offline · last attempt failed", dot: "bg-[#c2410c]" },
};

// Server component: fetches and picks one of three states.
export default async function Leaderboard() {
  const data = await getLeaderboard();
  const status = STATUS[data.status];

  return (
    <section
      id="leaderboard"
      className="relative border-t border-line py-24 sm:py-32"
      aria-labelledby="lb-title"
    >
      <div className="mx-auto max-w-[1320px] px-5 sm:px-8">
        <div className="grid gap-8 lg:grid-cols-[1fr_auto] lg:items-end">
          <div>
            <Kicker>Leaderboard</Kicker>
            <h2
              id="lb-title"
              className="mt-5 text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]"
            >
              Who&apos;s saving <Accent>the most.</Accent>
            </h2>
            <p className="mt-5 max-w-[52ch] text-lg leading-relaxed text-ink-soft">
              Ranked by energy saved in the game. The board updates whenever a player&apos;s
              progress syncs.
            </p>
          </div>
          <p className="font-mono text-xs text-ink-soft">
            <span
              className={`mr-2 inline-block h-1.5 w-1.5 rounded-full align-middle ${status.dot}`}
            />
            {status.text}
          </p>
        </div>

        <StatFigures
          stats={data.status === "error" ? null : data.stats}
          muted={data.status !== "ok"}
        />

        <div className="mt-12">
          {data.status === "ok" && (
            <>
              <LeaderboardTable entries={data.entries} />
              <PlayerSearch />
            </>
          )}
          {data.status === "empty" && <LeaderboardEmpty />}
          {data.status === "error" && <LeaderboardError />}
        </div>
      </div>
    </section>
  );
}
