import Link from "next/link";
import Kicker from "@/app/components/ui/Kicker";
import { ARTICLES } from "@/lib/articles";
import GuideTile from "./GuideTile";

// Landing-page preview of the guides. Sits at the bottom of the #learn band.
export default function GuidesStrip() {
  return (
    <section
      id="guides"
      className="relative py-24 sm:py-32"
      aria-labelledby="guides-title"
    >
      <div className="mx-auto max-w-[1320px] px-5 sm:px-8">
        <div className="flex flex-wrap items-end justify-between gap-4">
          <div>
            <Kicker>Guides</Kicker>
            <h3
              id="guides-title"
              className="mt-4 text-2xl font-semibold tracking-tight text-ink sm:text-3xl"
            >
              Go further, at home.
            </h3>
          </div>
          <Link href="/learn" className="group font-medium text-ink">
            All guides{" "}
            <span
              className="inline-block transition-transform group-hover:translate-x-1"
              aria-hidden="true"
            >
              →
            </span>
          </Link>
        </div>

        <ol className="mt-10 grid gap-x-8 gap-y-12 sm:grid-cols-2 lg:grid-cols-4">
          {ARTICLES.map((g, i) => (
            <GuideTile key={g.slug} guide={g} index={i + 1} />
          ))}
        </ol>
      </div>
    </section>
  );
}
