import Link from "next/link";
import Icon from "@/app/components/ui/Icon";

// Large row used on the /learn index.
export default function GuideRow({ guide, index }) {
  return (
    <li>
      <Link
        href={`/learn/${guide.slug}`}
        className="group grid grid-cols-[auto_1fr] gap-x-6 gap-y-2 border-t border-line py-9 sm:grid-cols-[5rem_1fr_auto] sm:items-center"
      >
        <span className="font-pixel text-[28px] leading-none text-ink-soft group-hover:text-brand">
          {String(index).padStart(2, "0")}
        </span>
        <span>
          <span className="flex items-center gap-3">
            <Icon name={guide.icon} className="h-5 w-5 text-brand" />
            <span className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
              {guide.readTime} read
            </span>
          </span>
          <span className="mt-3 block text-2xl font-semibold leading-tight tracking-tight text-ink group-hover:underline group-hover:decoration-1 group-hover:underline-offset-[6px] sm:text-[30px]">
            {guide.title}
          </span>
          <span className="mt-2 block max-w-[60ch] text-lg leading-relaxed text-ink-soft">
            {guide.summary}
          </span>
        </span>
        <span className="col-start-2 font-medium text-ink sm:col-start-3">
          Read{" "}
          <span
            className="inline-block transition-transform group-hover:translate-x-1"
            aria-hidden="true"
          >
            →
          </span>
        </span>
      </Link>
    </li>
  );
}
