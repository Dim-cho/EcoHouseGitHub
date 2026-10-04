import Link from "next/link";
import { notFound } from "next/navigation";
import { ARTICLES, getArticle } from "@/lib/articles";
import { getSession } from "@/lib/session";
import { hasClaimed, ECO_POINTS_PER_POST } from "@/lib/claims";
import SiteNav from "@/app/components/SiteNav";
import Icon from "@/app/components/ui/Icon";
import ClaimEcoPoints from "@/app/components/blog/ClaimEcoPoints";

// Reads the session to show claim state, so it can't be prerendered.
export const dynamic = "force-dynamic";

export function generateStaticParams() {
  return ARTICLES.map((a) => ({ slug: a.slug }));
}

export async function generateMetadata({ params }) {
  const { slug } = await params;
  const article = getArticle(slug);
  if (!article) return {};

  return {
    title: `${article.title} · Eco-House`,
    description: article.summary,
  };
}

export default async function ArticlePage({ params }) {
  const { slug } = await params;
  const article = getArticle(slug);
  if (!article) notFound();

  const index = ARTICLES.findIndex((a) => a.slug === slug);
  // Wraps, so the last guide still offers somewhere to go.
  const next = ARTICLES[(index + 1) % ARTICLES.length];

  const session = await getSession();
  const claimed = session ? await hasClaimed(session.username, slug) : false;

  return (
    <main>
      <SiteNav />

      <header className="relative overflow-hidden border-b border-line bg-linear-to-b from-sky to-paper">
        <div className="grain pointer-events-none absolute inset-0" />
        <div className="relative mx-auto max-w-[760px] px-5 pt-14 pb-14 sm:px-8 sm:pt-20">
          <Link
            href="/learn"
            className="font-mono text-[13px] uppercase tracking-[.18em] text-ink-soft transition-colors hover:text-ink"
          >
            ← Blog
          </Link>

          <p className="mt-8 flex items-center gap-3 font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
            <Icon name={article.icon} className="h-4 w-4 text-brand" />
            Guide {String(index + 1).padStart(2, "0")} · {article.readTime} read
          </p>

          <h1 className="mt-4 text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]">
            {article.title}
          </h1>
          <p className="mt-5 text-xl leading-relaxed text-ink-soft">{article.summary}</p>
        </div>
      </header>

      <article className="mx-auto max-w-[760px] px-5 py-16 sm:px-8 sm:py-20">
        <div className="space-y-6 text-[18px] leading-[1.75] text-ink/90">
          {article.sections.map((s) => (
            <section key={s.heading}>
              <h2 className="pt-6 text-2xl font-semibold tracking-tight text-ink">{s.heading}</h2>
              <p className="mt-3">{s.body}</p>
            </section>
          ))}
        </div>

        {article.inGame && (
          <aside className="mt-12 rounded-xl border border-line bg-band p-6">
            <p className="font-mono text-[13px] uppercase tracking-[.16em] text-brand">In the game</p>
            <p className="mt-2 text-ink">{article.inGame}</p>
          </aside>
        )}

        <div className="mt-16">
          <ClaimEcoPoints
            slug={slug}
            points={ECO_POINTS_PER_POST}
            signedIn={Boolean(session)}
            alreadyClaimed={claimed}
          />
        </div>

        <Link
          href={`/learn/${next.slug}`}
          className="group mt-10 flex items-center justify-between gap-6 border-t border-line pt-6"
        >
          <span>
            <span className="font-mono text-[13px] uppercase tracking-[.16em] text-ink-soft">
              Next guide
            </span>
            <span className="mt-1 block text-lg font-semibold tracking-tight text-ink transition-colors group-hover:text-brand">
              {next.title}
            </span>
          </span>
          <span className="transition-transform group-hover:translate-x-1" aria-hidden="true">
            →
          </span>
        </Link>
      </article>
    </main>
  );
}
