import LeaderboardHead from "./LeaderboardHead";

const fmt = (n) => n.toLocaleString("en-US");

export default function LeaderboardTable({ entries }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full">
        <caption className="sr-only">Top 10 players by energy saved</caption>
        <LeaderboardHead />
        <tbody>
          {entries.slice(0, 10).map((e) => (
            <tr key={e.username} className="border-t border-line">
              <td
                className={`py-3.5 pr-4 font-pixel text-lg ${
                  e.rank <= 3 ? "text-brand" : "text-ink-soft"
                }`}
              >
                {String(e.rank).padStart(2, "0")}
              </td>
              <td className="py-3.5 pr-4 font-medium text-ink">{e.username}</td>
              <td className="hidden py-3.5 pr-6 text-right font-mono text-sm tabular-nums text-ink-soft sm:table-cell">
                {fmt(e.co2Kg)} kg
              </td>
              <td className="py-3.5 text-right font-mono text-sm tabular-nums text-ink">
                {fmt(e.kwhSaved)} kWh
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
