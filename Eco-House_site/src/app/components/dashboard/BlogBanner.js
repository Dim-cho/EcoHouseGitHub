import Link from "next/link";
import Icon from "@/app/components/ui/Icon";

export default function BlogBanner({ guide, points }) {
  return (
    <Link
      href={`/learn/${guide.slug}`}
      className="group relative block overflow-hidden rounded-xl bg-ink p-7 text-paper sm:p-9"
    >
      <div className="dots pointer-events-none absolute inset-y-0 right-0 w-1/2 opacity-30 invert" />

      <div className="relative grid gap-6 sm:grid-cols-[auto_1fr_auto] sm:items-center">
        <span className="flex h-14 w-14 items-center justify-center rounded-lg bg-paper/10 text-[#9fe0b8]">
          <Icon name={guide.icon} className="h-8 w-8" strokeWidth={1.5} />
        </span>
        <div>
          <p className="font-mono text-[13px] uppercase tracking-[.16em] text-paper/60">
            From the blog · {guide.readTime} read
          </p>
          <p className="mt-2 text-xl font-semibold leading-snug tracking-tight sm:text-2xl">
            {guide.title}
          </p>
        </div>
        <span className="inline-flex items-center gap-3 justify-self-start rounded-lg bg-paper px-5 py-3 font-medium text-ink sm:justify-self-end">
          Read &amp; earn <span className="font-pixel text-brand">+{points}</span>
          <span className="transition-transform group-hover:translate-x-1" aria-hidden="true">
            →
          </span>
        </span>
      </div>
    </Link>
  );
}
