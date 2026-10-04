export default function LeaderboardHead() {
  return (
    <thead>
      <tr className="text-left font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
        <th className="w-14 pb-3 font-normal">Rank</th>
        <th className="pb-3 font-normal">Player</th>
        <th className="hidden pb-3 pr-6 text-right font-normal sm:table-cell">CO₂ avoided</th>
        <th className="pb-3 text-right font-normal">Energy saved</th>
      </tr>
    </thead>
  );
}
