import Link from "next/link";

// No email promise here: accounts are username-only, so the site can't mail
// anyone at launch.
export default function AccountCard({ signedIn }) {
  if (signedIn) {
    return (
      <div className="mt-10 rounded-xl border border-line bg-paper p-6 sm:p-8">
        <p className="font-mono text-[13px] uppercase tracking-[.16em] text-brand">
          You&apos;re all set
        </p>
        <p className="mt-3 text-xl font-semibold tracking-tight text-ink">
          Your spot is reserved.
        </p>
        <p className="mt-2 leading-relaxed text-ink-soft">
          Log in to the game with this account and your progress lands on the leaderboard.
        </p>
      </div>
    );
  }

  return (
    <div className="mt-10 rounded-xl border border-ink/80 bg-paper p-6 sm:p-8">
      <p className="font-mono text-[13px] uppercase tracking-[.16em] text-brand">
        What you can do now
      </p>
      <p className="mt-3 text-xl font-semibold tracking-tight text-ink">Create your account.</p>
      <p className="mt-2 leading-relaxed text-ink-soft">
        It&apos;s the same login in the game and on the site, and it&apos;s what puts your progress
        on the leaderboard when you start playing.
      </p>
      <div className="mt-6 flex flex-wrap items-center gap-x-6 gap-y-3">
        <Link
          href="/signup"
          className="inline-flex items-center gap-2 rounded-lg bg-ink px-6 py-3.5 font-medium text-paper transition-colors hover:bg-ink/85"
        >
          Create an account
        </Link>
        <Link
          href="/login"
          className="font-medium text-ink underline decoration-line decoration-1 underline-offset-4 transition-colors hover:decoration-ink"
        >
          I already have one
        </Link>
      </div>
    </div>
  );
}
