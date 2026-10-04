import Kicker from "@/app/components/ui/Kicker";
import Accent from "@/app/components/ui/Accent";
import PlatformRow from "./PlatformRow";
import AccountCard from "./AccountCard";
import BuildStatus from "./BuildStatus";

const PLATFORMS = [
  { name: "Windows", requirements: "Windows 10 or later · 64-bit", icon: "windows" },
  { name: "macOS", requirements: "Apple Silicon & Intel", icon: "apple" },
  { name: "Linux", requirements: "AppImage · x86_64", icon: "linux" },
];

export default function DownloadSection({ signedIn = false }) {
  return (
    <section
      id="download"
      className="relative border-t border-line py-24 sm:py-32"
      aria-labelledby="dl-title"
    >
      <div className="mx-auto grid max-w-[1320px] gap-14 px-5 sm:px-8 lg:grid-cols-2 lg:gap-20">
        <div>
          <Kicker>Download</Kicker>
          <h2
            id="dl-title"
            className="mt-5 text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]"
          >
            Not out yet.
            <br />
            <Accent>Be ready when it is.</Accent>
          </h2>
          <p className="mt-5 max-w-[46ch] text-lg leading-relaxed text-ink-soft">
            Eco-House is built in Unity and will be free while it&apos;s in development.
            There&apos;s nothing to download today, so we won&apos;t pretend there is.
          </p>
          <AccountCard signedIn={signedIn} />
        </div>

        <div className="lg:pt-16">
          <ul className="border-b border-line" aria-label="Platforms">
            {PLATFORMS.map((p) => (
              <PlatformRow key={p.name} platform={p} />
            ))}
          </ul>
          <BuildStatus />
        </div>
      </div>
    </section>
  );
}
