const TVS = [
  { tier: 1, src: "/tv/tv-1-old-crt.svg", alt: "An old CRT television in a boxy wooden cabinet" },
  { tier: 2, src: "/tv/tv-2-modern-led.svg", alt: "A thin modern LED television" },
  {
    tier: 3,
    src: "/tv/tv-3-future-hover.svg",
    alt: "A frameless television hovering in mid-air, lit from below",
  },
];

// The two unchosen tiers step back in chronological order, chosen one in front.
function slotFor(tier, active) {
  const order = [1, 2, 3].filter((t) => t !== active).concat(active);
  return order.indexOf(tier);
}

export default function TvStack({ active, onSelect }) {
  return (
    // 10/9 matches the SVGs' own 400x360, so object-contain doesn't letterbox.
    // Left-aligned from lg up: the deck steps up-right, so anchoring left keeps
    // that growth inside the column.
    <div className="stack relative mx-auto aspect-[10/9] w-full max-w-[340px] sm:max-w-[400px] lg:mt-[140px] lg:mr-auto lg:ml-0 lg:max-w-[360px] xl:max-w-[400px]">
      {TVS.map((tv) => (
        <button
          key={tv.tier}
          type="button"
          className="tv absolute inset-0 block"
          data-slot={slotFor(tv.tier, active)}
          onClick={() => onSelect(tv.tier)}
          aria-label={`Show ${tv.alt}`}
        >
          {/* Plain <img> so the SVG's own animations keep running. */}
          <img src={tv.src} alt={tv.alt} className="h-full w-full object-contain" />
        </button>
      ))}
    </div>
  );
}
