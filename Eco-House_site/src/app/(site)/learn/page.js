import { ARTICLES } from "@/lib/articles";
import SiteNav from "@/app/components/SiteNav";
import Kicker from "@/app/components/ui/Kicker";
import Accent from "@/app/components/ui/Accent";
import GuideRow from "@/app/components/guides/GuideRow";

export const metadata = {
  title: "Guides · Eco-House",
  description:
    "Where household energy actually goes, and which changes are worth making first.",
};

export default function LearnPage() {
  return (
    <main className="flex-1">
      <SiteNav />
      <section className="py-24 sm:py-32">
        <div className="mx-auto max-w-[1320px] px-5 sm:px-8">
          <Kicker>Guides</Kicker>
          <h1 className="mt-5 max-w-[18ch] text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]">
            The same ideas, <Accent>at real scale.</Accent>
          </h1>
          <p className="mt-5 max-w-[52ch] text-lg leading-relaxed text-ink-soft">
            The game compresses years into an afternoon. These are the same ideas at real scale —
            what actually uses the power, and which fixes are worth doing first.
          </p>

          <ol className="mt-16 border-b border-line">
            {ARTICLES.map((a, i) => (
              <GuideRow key={a.slug} guide={a} index={i + 1} />
            ))}
          </ol>

          <p className="mt-10 max-w-[60ch] font-mono text-xs leading-relaxed text-ink-soft">
            Figures are typical European household averages and vary by home, climate, and tariff.
            They are here to show the order of magnitude, not to price your specific bill.
          </p>
        </div>
      </section>
    </main>
  );
}
