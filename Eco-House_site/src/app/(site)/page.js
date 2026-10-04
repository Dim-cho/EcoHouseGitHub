import { getSession } from "@/lib/session";
import SiteNav from "@/app/components/SiteNav";
import Hero from "@/app/components/hero/Hero";
import Leaderboard from "@/app/components/leaderboard/Leaderboard";
import EnergySection from "@/app/components/learn/EnergySection";
import GuidesStrip from "@/app/components/guides/GuidesStrip";
import DownloadSection from "@/app/components/download/DownloadSection";

export const dynamic = "force-dynamic";

export default async function Home() {
  const session = await getSession();

  return (
    <main className="flex-1">
      <SiteNav floating />
      <Hero />
      <Leaderboard />

      {/* One band: the explanation and the guides that follow from it. */}
      <div id="learn" className="scroll-mt-20 border-t border-line bg-band">
        <EnergySection />
        <GuidesStrip />
      </div>

      <DownloadSection signedIn={Boolean(session)} />
    </main>
  );
}
