import Link from "next/link";
import GreenScene from "./GreenScene";

// Soft sage background with faint contour lines and low hills; the form floats
// in the middle.
export default function AuthLayout({ children }) {
  return (
    <div className="relative isolate flex min-h-[100svh] flex-col overflow-hidden">
      <div className="absolute inset-0 -z-20 bg-linear-to-b from-[#eef3ec] via-[#f1f4ee] to-[#e6eee3]" />
      <div className="grain pointer-events-none absolute inset-0 -z-10" />
      <GreenScene />

      <header className="relative flex items-center justify-between px-5 py-5 sm:px-8">
        <Link href="/" aria-label="Eco-House home">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src="/logo/ecohouse-logo.svg"
            alt="Eco-House"
            className="h-9 w-auto"
            style={{ imageRendering: "pixelated" }}
          />
        </Link>
        <Link
          href="/"
          className="font-mono text-[13px] uppercase tracking-[.18em] text-ink-soft transition-colors hover:text-ink"
        >
          ← Back to site
        </Link>
      </header>

      <main className="relative flex flex-1 items-start justify-center px-5 pt-2 pb-[max(26svh,180px)] sm:items-center">
        <div className="w-full max-w-[420px] rounded-2xl border border-[#d5e0d2] bg-paper/95 p-6 shadow-[0_24px_60px_-34px_rgb(31_122_82/.45)] backdrop-blur sm:p-8">
          {children}
        </div>
      </main>
    </div>
  );
}
