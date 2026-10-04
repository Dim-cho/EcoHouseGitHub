// Mono micro-label: "01 —— LEADERBOARD"
export default function Kicker({ n, children }) {
  return (
    <p className="flex items-center gap-3 font-mono text-xs uppercase tracking-[.18em] text-ink-soft">
      {n && <span className="text-ink">{n}</span>}
      <span className="h-px w-8 bg-ink-soft/60" />
      {children}
    </p>
  );
}
