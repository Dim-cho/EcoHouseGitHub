const fmt = (n) => n.toLocaleString("en-US");

// stats === null is the error state ("—"); muted greys the numbers when they
// aren't live yet.
export default function StatFigures({ stats, muted }) {
  const items = [
    { label: "Energy saved, all players", value: stats?.totalKwh, unit: "kWh" },
    { label: "CO₂ avoided", value: stats?.co2Kg, unit: "kg" },
    { label: "Trees working for a year", value: stats?.trees, unit: "trees" },
  ];

  return (
    <dl className="mt-14 grid divide-y divide-line border-y border-line sm:grid-cols-3 sm:divide-x sm:divide-y-0 sm:py-6">
      {items.map((it) => (
        <div key={it.label} className="py-6 sm:px-8 sm:py-2 sm:first:pl-0">
          <dt className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
            {it.label}
          </dt>
          <dd className="mt-3 flex items-baseline gap-2">
            <span
              className={`font-pixel text-[40px] leading-none sm:text-[48px] ${
                muted ? "text-ink-soft/55" : "text-ink"
              }`}
            >
              {it.value == null ? "—" : fmt(it.value)}
            </span>
            <span className="font-mono text-xs text-ink-soft">{it.unit}</span>
          </dd>
        </div>
      ))}
    </dl>
  );
}
