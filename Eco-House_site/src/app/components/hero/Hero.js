"use client";

import { useEffect, useState } from "react";
import Landscape from "./Landscape";
import TierSelector from "./TierSelector";
import TvStack from "./TvStack";

const DURATION = 3600;

export default function Hero() {
  // Reduced motion means no cycling at all, so start on the tier the hero
  // should rest at. Read in the initialiser so there's no corrective re-render.
  const [tier, setTier] = useState(() =>
    typeof window !== "undefined" &&
    window.matchMedia("(prefers-reduced-motion: reduce)").matches
      ? 3
      : 1,
  );
  const [autoplaying, setAutoplaying] = useState(
    () =>
      typeof window === "undefined" ||
      !window.matchMedia("(prefers-reduced-motion: reduce)").matches,
  );

  useEffect(() => {
    if (!autoplaying) return;

    const id = setInterval(() => setTier((t) => (t % 3) + 1), DURATION);
    return () => clearInterval(id);
  }, [autoplaying]);

  // Any deliberate interaction ends autoplay for the rest of the visit.
  function select(next) {
    setAutoplaying(false);
    setTier(next);
  }

  return (
    <section
      id="top"
      className="hero relative isolate flex min-h-[auto] flex-col overflow-hidden lg:min-h-[100svh]"
      data-state={tier}
    >
      <div className="absolute inset-0 -z-20 bg-linear-to-b from-sky to-paper" />
      <div
        className="absolute inset-0 -z-20 bg-linear-to-b from-smog to-paper/0 transition-opacity duration-1000"
        style={{ opacity: "var(--haze)" }}
      />
      <div className="grain pointer-events-none absolute inset-0 -z-10" />

      <Landscape />

      <div className="relative mx-auto grid w-full max-w-[1320px] flex-1 items-center gap-8 px-5 pt-24 pb-32 sm:gap-10 sm:px-8 sm:pb-40 lg:grid-cols-[1fr_1.25fr] lg:gap-6 lg:pt-20 lg:pb-48">
        {/* relative + z-10 so the headline can run past its column into the
            TV column's empty left edge without the grid resizing. */}
        <div className="relative z-10 max-w-[640px]">
          {/* nowrap only from lg up — at 44px on a 360px phone the line is
              wider than the viewport and has to be allowed to break. */}
          {/* w-max on large screens lets the headline size to its text and
              spill into the gap, instead of wrapping inside the column. */}
          <h1 className="rise rise-1 text-[44px] font-semibold leading-[.98] tracking-[-.035em] text-ink sm:text-[60px] lg:w-max xl:text-[72px]">
            <span className="lg:whitespace-nowrap">Upgrade the house.</span>
            <br />
            <span className="relative inline-block font-serif text-[1.08em] font-normal italic tracking-[-.01em] text-brand">
              Clear the sky.
              <svg
                className="absolute -bottom-2 left-0 h-3 w-full text-brand/40"
                viewBox="0 0 300 12"
                preserveAspectRatio="none"
                aria-hidden="true"
              >
                <path
                  d="M2 8 C60 2 140 2 200 6 C240 8 270 7 298 4"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="3"
                  strokeLinecap="round"
                />
              </svg>
            </span>
          </h1>

          <p className="rise rise-2 mt-7 max-w-[46ch] text-lg leading-relaxed text-ink-soft">
            Eco-House is an idle game about cutting a home&apos;s energy bill to nothing. Swap out
            old appliances, bank every watt, and watch the world outside get cleaner, even while
            the game is closed.
          </p>

          <div className="rise rise-3 mt-9 flex flex-wrap items-center gap-x-7 gap-y-4">
            <a
              href="#download"
              className="inline-flex items-center gap-2.5 rounded-lg bg-ink px-6 py-4 font-medium text-paper transition-colors hover:bg-ink/85"
            >
              Download the game
              <span className="rounded bg-paper/15 px-1.5 py-0.5 font-mono text-[11px] uppercase tracking-wider">
                Soon
              </span>
            </a>
            <a href="#learn" className="group inline-flex items-center gap-2 font-medium text-ink">
              How it works
              <span className="transition-transform group-hover:translate-x-1" aria-hidden="true">
                →
              </span>
            </a>
          </div>

          <TierSelector
            active={tier}
            autoplaying={autoplaying}
            duration={DURATION}
            onSelect={select}
          />
        </div>

        <div className="fade-in relative">
          <div className="dots pointer-events-none absolute inset-0 -z-10" />
          <TvStack active={tier} onSelect={select} />
        </div>
      </div>
    </section>
  );
}
