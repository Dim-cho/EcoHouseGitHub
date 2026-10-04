"use client";

import { useEffect, useRef, useState } from "react";
import { SPONSORS } from "@/lib/sponsors";
import SponsorCard from "./SponsorCard";

export default function SponsorRow() {
  const trackRef = useRef(null);
  const [atStart, setAtStart] = useState(true);
  const [atEnd, setAtEnd] = useState(false);

  // Disable the arrows at each end rather than letting them do nothing.
  function sync() {
    const el = trackRef.current;
    if (!el) return;

    setAtStart(el.scrollLeft <= 1);
    setAtEnd(el.scrollLeft + el.clientWidth >= el.scrollWidth - 1);
  }

  useEffect(() => {
    sync();
    window.addEventListener("resize", sync);
    return () => window.removeEventListener("resize", sync);
  }, []);

  function scrollBy(direction) {
    const el = trackRef.current;
    if (!el) return;

    // One card plus its gap, so a click lands cards on the same rhythm.
    el.scrollBy({ left: direction * (el.clientWidth * 0.8), behavior: "smooth" });
  }

  return (
    <section aria-labelledby="aff-title">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <p className="font-mono text-[13px] uppercase tracking-[.18em] text-ink-soft">
            For your real home
          </p>
          <h2 id="aff-title" className="mt-2 text-xl font-semibold tracking-tight text-ink sm:text-2xl">
            Upgrades that match the game
          </h2>
          {/* Shown here on small screens, where the desktop position is hidden. */}
          <p className="mt-1 font-mono text-[13px] text-ink-soft lg:hidden">
            Suggestions only · we earn nothing from these
          </p>
        </div>

        <div className="flex items-center gap-4">
          {/* Real product links, but no affiliate programme behind them yet.
              When one exists this must change to disclose the commission —
              that's a legal requirement, not a nicety. */}
          <p className="hidden font-mono text-[13px] text-ink-soft lg:block">
            Suggestions only · we earn nothing from these
          </p>
          {/* Arrows are desktop-only: on a phone the row is swiped, and two
              buttons here squeeze the heading onto its own line. */}
          <div className="hidden gap-2 sm:flex">
            <button
              type="button"
              onClick={() => scrollBy(-1)}
              disabled={atStart}
              aria-label="Previous products"
              className="flex h-10 w-10 items-center justify-center rounded-full border border-line text-ink transition-colors hover:border-ink disabled:opacity-30 disabled:hover:border-line"
            >
              ←
            </button>
            <button
              type="button"
              onClick={() => scrollBy(1)}
              disabled={atEnd}
              aria-label="Next products"
              className="flex h-10 w-10 items-center justify-center rounded-full border border-line text-ink transition-colors hover:border-ink disabled:opacity-30 disabled:hover:border-line"
            >
              →
            </button>
          </div>
        </div>
      </div>

      <div
        ref={trackRef}
        onScroll={sync}
        className="no-scrollbar mt-6 snap-x snap-mandatory overflow-x-auto scroll-smooth"
      >
        {/* The cards are shrink-0, so the row overflows and scrolls. Don't put
            an explicit width here: it would stretch the page instead. */}
        <ul className="flex gap-4">
          {SPONSORS.map((s) => (
            <SponsorCard key={s.name} item={s} />
          ))}
        </ul>
      </div>
    </section>
  );
}
