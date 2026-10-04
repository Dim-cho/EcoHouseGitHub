export default function WasteBar({ wastedPct }) {
  const used = 100 - wastedPct;

  return (
    <figure className="mt-10">
      <figcaption className="font-mono text-[11px] uppercase tracking-[.14em] text-ink-soft">
        Every 100 units of electricity a home buys
      </figcaption>

      <div
        className="mt-3 flex h-12 overflow-hidden rounded-md border border-ink/80"
        role="img"
        aria-label={`Of every 100 units of electricity a home buys, about ${used} do useful work and about ${wastedPct} are wasted`}
      >
        <div
          className="flex flex-col justify-center bg-paper px-3"
          style={{ width: `${used}%` }}
        >
          <span className="font-pixel text-sm leading-none text-ink">{used}</span>
          <span className="mt-1 font-mono text-[12px] uppercase tracking-[.12em] text-ink-soft">
            Does something
          </span>
        </div>
        <div
          className="waste flex flex-col justify-center border-l border-ink/80 px-3"
          style={{ width: `${wastedPct}%` }}
        >
          <span className="font-pixel text-sm leading-none text-ink">{wastedPct}</span>
          <span className="mt-1 font-mono text-[12px] uppercase tracking-[.12em] text-ink">
            Wasted
          </span>
        </div>
      </div>

      <p className="mt-3 text-base leading-relaxed text-ink-soft">
        Those {wastedPct} units are paid for and do nothing — they leak out as standby draw, heat
        through the walls, and light that costs five times what it should.
      </p>
    </figure>
  );
}
