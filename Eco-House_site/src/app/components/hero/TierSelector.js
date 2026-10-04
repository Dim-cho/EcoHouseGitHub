const TIERS = [
  { tier: 1, num: "01", name: "Then", blurb: "Bulky, hot, always warm" },
  { tier: 2, num: "02", name: "Now", blurb: "Thin and efficient" },
  { tier: 3, num: "03", name: "Next", blurb: "Big, smart, sips power" },
];

export default function TierSelector({ active, autoplaying, duration, onSelect }) {
  return (
    <ol
      className="rise rise-4 mt-10 grid max-w-xl grid-cols-3 gap-4 sm:mt-12 sm:gap-6"
      aria-label="Upgrade tiers"
    >
      {TIERS.map((t) => {
        const on = t.tier === active;

        // The bar fills over the autoplay interval; a manual pick shows it full.
        const bar = on
          ? autoplaying
            ? { width: "100%", transition: `width ${duration}ms linear` }
            : { width: "100%", transition: "none" }
          : { width: "0%", transition: "none" };

        return (
          <li key={t.tier}>
            <button
              type="button"
              aria-pressed={on}
              onClick={() => onSelect(t.tier)}
              onMouseEnter={() => onSelect(t.tier)}
              className="tier group relative block w-full pt-4 text-left"
            >
              <span className="absolute inset-x-0 top-0 h-px bg-line">
                <span
                  key={`${t.tier}-${active}-${autoplaying}`}
                  className="block h-full bg-ink"
                  style={bar}
                />
              </span>
              <span className="font-mono text-[13px] text-ink-soft">{t.num}</span>
              <span className="mt-1 block text-[15px] font-semibold text-ink-soft transition-colors group-hover:text-ink group-aria-pressed:text-ink sm:text-base">
                {t.name}
              </span>
              <span className="mt-1 block text-[12px] leading-snug text-ink-soft sm:text-[13px]">
                {t.blurb}
              </span>
            </button>
          </li>
        );
      })}
    </ol>
  );
}
