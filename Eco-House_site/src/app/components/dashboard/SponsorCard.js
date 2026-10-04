import Icon from "@/app/components/ui/Icon";

export default function SponsorCard({ item }) {
  return (
    <li className="w-[280px] shrink-0 snap-start sm:w-[310px]">
      <a
        href={item.url}
        target="_blank"
        // sponsored+nofollow is required of affiliate links and harmless on
        // plain ones, so it's set either way.
        rel="sponsored nofollow noopener"
        className="group flex h-full flex-col rounded-xl border border-line bg-paper p-5 transition-colors hover:border-ink/40"
      >
        <span className="flex h-28 items-center justify-center rounded-lg bg-band text-brand">
          <Icon name={item.icon} className="h-12 w-12" strokeWidth={1.4} />
        </span>

        <span className="mt-4 font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
          {item.category}
        </span>
        <span className="mt-1 text-lg font-semibold tracking-tight text-ink">{item.name}</span>
        <span className="mt-1 text-base leading-relaxed text-ink-soft">{item.blurb}</span>

        <span className="mt-auto border-t border-line pt-4 font-mono text-[13px] leading-relaxed text-brand">
          {item.saving}
        </span>

        <span className="mt-3 flex items-center justify-between text-[15px] font-medium text-ink">
          View product
          <span
            className="transition-transform group-hover:translate-x-0.5 group-hover:-translate-y-0.5"
            aria-hidden="true"
          >
            ↗
          </span>
        </span>
      </a>
    </li>
  );
}
