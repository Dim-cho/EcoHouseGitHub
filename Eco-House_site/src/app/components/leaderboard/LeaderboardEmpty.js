import Link from "next/link";
import LeaderboardHead from "./LeaderboardHead";

const WIDTHS = ["w-32", "w-24", "w-28", "w-20", "w-36", "w-24", "w-28", "w-20", "w-32", "w-24"];

// Ten open places rather than an empty table: the shape of the board is the
// invitation.
export default function LeaderboardEmpty() {
  return (
    <div className="relative">
      <table className="w-full" aria-hidden="true">
        <LeaderboardHead />
        <tbody>
          {WIDTHS.map((w, i) => (
            <tr key={i} className="border-t border-dashed border-line">
              <td className="py-3.5 pr-4 font-pixel text-lg text-ink-soft/50">
                {String(i + 1).padStart(2, "0")}
              </td>
              <td className="py-3.5 pr-4">
                <span className={`block h-2 ${w} max-w-full rounded-sm bg-line`} />
              </td>
              <td className="hidden py-3.5 pr-6 sm:table-cell">
                <span className="ml-auto block h-2 w-14 rounded-sm bg-line/70" />
              </td>
              <td className="py-3.5">
                <span className="ml-auto block h-2 w-16 rounded-sm bg-line/70" />
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="absolute inset-x-0 top-24 flex justify-center px-2 sm:top-28">
        <div className="max-w-md rounded-xl border border-line bg-paper px-6 py-7 text-center sm:px-9">
          <p className="font-pixel text-sm uppercase tracking-wider text-brand">10 places open</p>
          <p className="mt-3 text-xl font-semibold tracking-tight text-ink">
            Nobody&apos;s on the board yet.
          </p>
          <p className="mt-2 text-ink-soft">
            Rankings start with the first synced save. Make an account now and your name is ready
            to go when the game launches.
          </p>
          <Link
            href="/signup"
            className="mt-6 inline-flex items-center gap-2 rounded-lg bg-ink px-5 py-3 font-medium text-paper transition-colors hover:bg-ink/85"
          >
            Claim a spot
            <span aria-hidden="true">→</span>
          </Link>
        </div>
      </div>
    </div>
  );
}
