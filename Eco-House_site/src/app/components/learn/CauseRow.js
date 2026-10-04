import Icon from "@/app/components/ui/Icon";

export default function CauseRow({ n, icon, title, figure, figureLabel, text, game }) {
  return (
    <li className="grid grid-cols-[auto_1fr] gap-x-5 border-t border-line py-8 sm:grid-cols-[3.5rem_1fr_9rem] sm:gap-x-6">
      <span className="flex h-11 w-11 items-center justify-center rounded-full bg-brand font-mono text-sm text-paper">
        {n}
      </span>
      <div>
        <h3 className="flex items-center gap-2.5 text-xl font-semibold tracking-tight text-ink">
          <Icon name={icon} className="h-5 w-5 text-ink-soft" />
          {title}
        </h3>
        <p className="mt-2 max-w-[54ch] leading-relaxed text-ink-soft">{text}</p>
        <p className="mt-3 font-mono text-xs uppercase tracking-[.14em] text-brand">
          In the game · {game}
        </p>
      </div>
      <div className="col-start-2 mt-4 sm:col-start-3 sm:mt-0 sm:text-right">
        <p className="font-pixel text-[32px] leading-none text-ink">{figure}</p>
        <p className="mt-2 font-mono text-[13px] uppercase tracking-[.14em] text-ink-soft">
          {figureLabel}
        </p>
      </div>
    </li>
  );
}
