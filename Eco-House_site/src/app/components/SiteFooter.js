import Link from "next/link";
import { getSession } from "@/lib/session";

// Only routes that exist — a footer full of dead links is worse than a short one.
const COLUMNS = [
  {
    title: "Game",
    links: [
      ["/#download", "Download"],
      ["/#leaderboard", "Leaderboard"],
    ],
  },
  {
    title: "Learn",
    links: [
      ["/#learn", "Where the energy goes"],
      ["/learn", "All guides"],
    ],
  },
];

const ACCOUNT_OUT = [
  ["/signup", "Sign up"],
  ["/login", "Log in"],
];

const ACCOUNT_IN = [["/dashboard", "Dashboard"]];

export default async function SiteFooter() {
  const session = await getSession();
  const columns = [
    ...COLUMNS,
    { title: "Account", links: session ? ACCOUNT_IN : ACCOUNT_OUT },
  ];

  return (
    <footer className="relative border-t border-line bg-band">
      {/* Phones get the link columns side by side: stacked, three columns plus
          the logo block made for a very long, sparse footer. */}
      <div className="mx-auto grid max-w-[1320px] grid-cols-2 gap-x-6 gap-y-10 px-5 pt-14 pb-10 sm:grid-cols-3 sm:px-8 lg:grid-cols-[1.6fr_1fr_1fr_1fr] lg:gap-12 lg:pt-16">
        <div className="col-span-2 sm:col-span-3 lg:col-span-1">
          <Link href="/" aria-label="Eco-House home">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src="/logo/ecohouse-logo.svg"
              alt="Eco-House"
              className="h-9 w-auto"
              style={{ imageRendering: "pixelated" }}
            />
          </Link>
          <p className="mt-5 max-w-[34ch] text-base leading-relaxed text-ink-soft">
            An idle game about cutting a home&apos;s energy bill to nothing, and a few honest
            guides to doing it for real.
          </p>
        </div>

        {columns.map((col) => (
          <nav key={col.title} aria-label={col.title}>
            <p className="font-mono text-[13px] uppercase tracking-[.16em] text-ink">
              {col.title}
            </p>
            <ul className="mt-4 space-y-2.5 text-[15px]">
              {col.links.map(([href, label]) => (
                <li key={href}>
                  <Link href={href} className="text-ink-soft transition-colors hover:text-ink">
                    {label}
                  </Link>
                </li>
              ))}
            </ul>
          </nav>
        ))}
      </div>

      <div className="mx-auto flex max-w-[1320px] flex-col justify-between gap-3 border-t border-line px-5 py-6 font-mono text-xs text-ink-soft sm:flex-row sm:px-8">
        <span>© {new Date().getFullYear()} Eco-House</span>
        <span>Built in Unity · In-game figures are scaled for play</span>
      </div>
    </footer>
  );
}
