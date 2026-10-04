import Kicker from "@/app/components/ui/Kicker";
import Accent from "@/app/components/ui/Accent";
import WasteBar from "./WasteBar";
import HouseCutaway from "./HouseCutaway";
import CauseRow from "./CauseRow";
import ClosingNote from "./ClosingNote";

const CAUSES = [
  {
    n: 1,
    icon: "plug",
    title: "Standby power",
    figure: "5–10%",
    figureLabel: "of a typical bill",
    text: "TVs, consoles, chargers and boxes keep drawing power when they look switched off. Nothing happens; you still pay.",
    game: "smart strips, then auto-off",
  },
  {
    n: 2,
    icon: "bulb",
    title: "Lighting",
    figure: "−80%",
    figureLabel: "LED vs halogen",
    text: "Swapping halogen bulbs for LEDs gives the same light for about a fifth of the power, and they last far longer.",
    game: "LED retrofit",
  },
  {
    n: 3,
    icon: "house",
    title: "Heating & cooling",
    figure: "No. 1",
    figureLabel: "largest single draw",
    text: "The biggest load in most homes, and the one that depends most on insulation. Heat you pay for leaks out through roofs, walls and windows.",
    game: "seal → insulate → heat pump",
  },
];

export default function EnergySection() {
  return (
    <section className="relative pt-24 sm:pt-32" aria-labelledby="learn-title">
      <div className="mx-auto max-w-[1320px] px-5 sm:px-8">
        <div className="grid gap-14 lg:grid-cols-[1fr_1.1fr] lg:gap-16">
          <div className="lg:sticky lg:top-10 lg:self-start">
            <Kicker>Where the energy goes</Kicker>
            <h2
              id="learn-title"
              className="mt-5 text-[34px] font-semibold leading-[1.02] tracking-[-.03em] text-ink sm:text-[48px]"
            >
              A quarter of it
              <br />
              <Accent>does nothing.</Accent>
            </h2>
            <p className="mt-5 max-w-[46ch] text-lg leading-relaxed text-ink-soft">
              Roughly one unit in four of a home&apos;s electricity is wasted. Most of it
              disappears in three places, and none of them are hard to fix.
            </p>
            <WasteBar wastedPct={25} />
            <div className="mt-10 max-w-[480px]">
              <HouseCutaway />
            </div>
          </div>

          <div>
            <ol className="border-b border-line">
              {CAUSES.map((c) => (
                <CauseRow key={c.n} {...c} />
              ))}
            </ol>
            <ClosingNote />
          </div>
        </div>
      </div>
    </section>
  );
}
