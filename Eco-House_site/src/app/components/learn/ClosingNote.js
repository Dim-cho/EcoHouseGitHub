import Accent from "@/app/components/ui/Accent";

export default function ClosingNote() {
  return (
    <blockquote className="mt-12 border-l-2 border-brand pl-6">
      <p className="text-2xl font-semibold leading-snug tracking-tight text-ink">
        The game&apos;s numbers are scaled for play. <Accent>The order is real.</Accent>
      </p>
      <p className="mt-4 max-w-[52ch] leading-relaxed text-ink-soft">
        Upgrades unlock in roughly the order they pay for themselves at home. The cheapest fixes
        come first, and they&apos;re the ones people skip.
      </p>
    </blockquote>
  );
}
