import PlatformIcon from "@/app/components/ui/PlatformIcon";

// Set `platform.download = { url, version, size }` when a build ships.
export default function PlatformRow({ platform: p }) {
  return (
    <li className="grid grid-cols-[auto_1fr] items-center gap-x-5 gap-y-3 border-t border-line py-6 sm:grid-cols-[auto_1fr_auto]">
      <span className="flex h-12 w-12 items-center justify-center rounded-lg border border-line bg-paper text-ink">
        <PlatformIcon name={p.icon} className="h-6 w-6" />
      </span>
      <div>
        <p className="text-lg font-semibold tracking-tight text-ink">{p.name}</p>
        <p className="font-mono text-xs text-ink-soft">{p.requirements}</p>
      </div>
      {p.download ? (
        <a
          href={p.download.url}
          className="col-start-2 inline-flex items-center gap-2 justify-self-start rounded-lg bg-ink px-4 py-2.5 font-medium text-paper transition-colors hover:bg-ink/85 sm:col-start-3"
        >
          Download{" "}
          <span className="font-mono text-xs text-paper/70">
            v{p.download.version} · {p.download.size}
          </span>
        </a>
      ) : (
        <p className="col-start-2 flex items-center gap-2 sm:col-start-3">
          <span className="pulse-soft h-2 w-2 rounded-full bg-[#d9a12b]" />
          <span className="font-pixel text-sm uppercase tracking-wider text-ink-soft">
            Coming soon
          </span>
        </p>
      )}
    </li>
  );
}
