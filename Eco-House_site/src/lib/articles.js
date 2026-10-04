// Educational content. Plain data so /learn and /learn/[slug] stay in sync.
export const ARTICLES = [
  {
    slug: "standby-power",
    icon: "plug",
    title: "The power you pay for and never use",
    summary:
      "Devices that look off are rarely off. Standby draw is the cheapest waste to eliminate and the easiest to forget.",
    readTime: "3 min",
    sections: [
      {
        heading: "What standby actually costs",
        body: "A television, games console, printer, and a handful of chargers left plugged in typically draw somewhere between 30 and 100 watts around the clock. That is 260 to 875 kWh a year — in most European markets, the cost of a month or two of normal usage, spent on nothing at all.",
      },
      {
        heading: "Why devices do it",
        body: "Standby keeps a device listening: for a remote, for a network command, for a scheduled update. Some of that is genuinely useful. A set-top box that records overnight needs to stay awake. A phone charger with no phone attached does not.",
      },
      {
        heading: "What to do about it",
        body: "Group the devices that never need standby — chargers, speakers, the printer — onto one switched power strip and flip it when you leave the room. Leave genuinely scheduled devices alone. The point is not to unplug everything, it is to stop paying for the ones doing nothing.",
      },
    ],
    inGame: "Standby draw is the first meter you can reduce in Eco-House, and it is why the early game feels fast.",
  },
  {
    slug: "lighting",
    icon: "bulb",
    title: "Lighting: the upgrade that pays for itself twice",
    summary:
      "LEDs use about a fifth of the energy of halogen for the same light, and last roughly fifteen times as long.",
    readTime: "3 min",
    sections: [
      {
        heading: "The arithmetic",
        body: "A 50W halogen spot and a 9W LED produce roughly the same amount of light. Across twenty fittings run five hours a day, that difference is about 1,500 kWh a year. The bulbs cost more up front and are usually cheaper within a year.",
      },
      {
        heading: "Lumens, not watts",
        body: "Watts measure consumption, not brightness. When replacing a bulb, match the lumens: roughly 800 lm for a standard 60W incandescent, 1,500 lm for a 100W. Colour temperature is separate — 2700K reads as warm, 4000K as neutral, 6000K as daylight and tends to feel clinical indoors.",
      },
      {
        heading: "Where it goes wrong",
        body: "Cheap LEDs fail early, usually because the driver electronics overheat in an enclosed fitting. Check the fitting is rated for enclosed use if the bulb sits behind glass, and prefer a known brand for anything awkward to reach.",
      },
    ],
    inGame: "The LED retrofit is the first real upgrade tier, and the multiplier reflects the roughly 80% reduction.",
  },
  {
    slug: "heating-and-insulation",
    icon: "house",
    title: "Seal the house before you heat it",
    summary:
      "Heating is the largest single draw in most homes. Insulation decides how much of it you keep.",
    readTime: "4 min",
    sections: [
      {
        heading: "The biggest line on the bill",
        body: "In temperate climates, space heating and hot water commonly account for 60–70% of a household's total energy use. Every other saving in this guide is small by comparison — which is also why heating is where improvements are worth the most.",
      },
      {
        heading: "Cheap fixes first",
        body: "Draught-proofing strips around doors and windows, a brush on the letterbox, and a chimney balloon in an unused fireplace cost very little and typically pay for themselves within a single heating season. Loft insulation is the next rung: effective, and usually the last one a homeowner can do without a contractor.",
      },
      {
        heading: "Thermostat behaviour",
        body: "Lowering a thermostat by one degree reduces heating energy by roughly 5–10%. Heating an empty house is pure loss, so a timer or a smart thermostat matched to when people are actually home often saves more than any single piece of hardware.",
      },
    ],
    inGame: "Insulation upgrades in Eco-House reduce your draw rather than raising output — the same trade-off as the real thing.",
  },
  {
    slug: "appliances",
    icon: "fridge",
    title: "Which appliances actually matter",
    summary:
      "A handful of machines dominate the bill. Knowing which ones tells you where replacing is worth the money.",
    readTime: "3 min",
    sections: [
      {
        heading: "The heavy hitters",
        body: "Anything that heats or cools uses serious power: the tumble dryer, the oven, the kettle, the fridge, and the washing machine's heating element. Electronics — laptops, phones, routers — are trivial by comparison, however guilty they feel.",
      },
      {
        heading: "Washing cold",
        body: "Around 90% of a washing machine's energy goes into heating the water, not turning the drum. Modern detergents are formulated for 30°C and handle ordinary laundry fine. Dropping from 60°C to 30°C cuts that cycle's energy roughly in half.",
      },
      {
        heading: "Drying",
        body: "A tumble dryer is usually the single most expensive appliance in the house to run. A clothes airer costs nothing and takes a day. Where a dryer is necessary, a heat-pump model uses roughly half the energy of a conventional vented one.",
      },
    ],
    inGame: "Appliance upgrades appear in the mid game, when raw output growth starts to slow and efficiency matters more.",
  },
];

export function getArticle(slug) {
  return ARTICLES.find((a) => a.slug === slug) ?? null;
}
