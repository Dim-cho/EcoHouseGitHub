import Icon from "@/app/components/ui/Icon";

const fmt = (n) => n.toLocaleString("en-US");

function Stat({ icon, label, value, unit, note }) {
  return (
    <div className="py-6 sm:px-7 sm:first:pl-0">
      <dt className="flex items-center gap-2 font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
        <Icon name={icon} className="h-4 w-4 text-brand" />
        {label}
      </dt>
      <dd className="mt-3">
        {/* Two columns on a phone leave ~160px: a seven-figure total would
            overflow at the full size. */}
        <span className="font-pixel text-[30px] leading-none text-ink sm:text-[40px]">
          {value}
        </span>
        {unit && <span className="ml-2 font-mono text-sm text-ink-soft">{unit}</span>}
        {note && <p className="mt-2 text-[15px] text-ink-soft">{note}</p>}
      </dd>
    </div>
  );
}

export default function ProgressStats({ stats, rank, totalPlayers }) {
  const synced = stats.lastSync
    ? new Date(stats.lastSync).toLocaleString("en-GB", {
        day: "numeric",
        month: "short",
        hour: "2-digit",
        minute: "2-digit",
      })
    : null;

  return (
    <div>
      <dl className="grid grid-cols-2 divide-line border-y border-line lg:grid-cols-4 lg:divide-x">
        <Stat icon="coin" label="Coins" value={fmt(stats.coins)} />
        <Stat
          icon="leaf"
          label="Eco points"
          value={fmt(stats.ecoPoints)}
          note="Earned in game + from the blog"
        />
        <Stat icon="bolt" label="Energy saved" value={fmt(stats.kwhSaved)} unit="kWh" />
        <Stat
          icon="trophy"
          label="Rank"
          value={rank ? `#${rank}` : "—"}
          note={rank ? `of ${fmt(totalPlayers)} players` : "Not on the board yet"}
        />
      </dl>

      {synced && (
        <p className="mt-3 font-mono text-[13px] text-ink-soft">
          <span className="mr-2 inline-block h-1.5 w-1.5 rounded-full bg-brand align-middle" />
          Last synced from the game · {synced}
        </p>
      )}
    </div>
  );
}
