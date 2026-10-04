// Line icons, 24x24, drawn in currentColor. These replaced a 9x9 pixel-art set
// that stopped being legible below about 32px.
const PATHS = {
  plug: [
    "M9 2v6M15 2v6",
    "M6 8h12v3a6 6 0 0 1-6 6 6 6 0 0 1-6-6V8Z",
    "M12 17v5",
  ],
  bulb: [
    "M9 18h6M10 22h4",
    "M12 2a7 7 0 0 0-4 12.7V18h8v-3.3A7 7 0 0 0 12 2Z",
  ],
  house: ["M3 11 12 3l9 8", "M5 10v11h14V10", "M10 21v-6h4v6"],
  fridge: ["M6 2h12v20H6z", "M6 10h12", "M9 6v2M9 13v2"],
  strip: ["M3 9h18v6H3z", "M7 9V6M17 9V6", "M7 12h.01M12 12h.01M17 12h.01"],
  thermo: [
    "M14 14.8V4a2 2 0 1 0-4 0v10.8a5 5 0 1 0 4 0Z",
    "M12 17.5a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3Z",
  ],
  door: ["M5 3h14v18H5z", "M9 7h6v10H9z", "M13.5 12h.01"],
  meter: ["M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18Z", "M12 12 15.5 8.5", "M12 12h.01"],
  coin: ["M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18Z", "M12 7v10", "M14.5 9.5h-4a1.75 1.75 0 0 0 0 3.5h3a1.75 1.75 0 0 1 0 3.5h-4"],
  leaf: [
    "M20 4C9 4 4 9.5 4 16c0 1.6.4 3 1 4",
    "M5 20c10 0 15-5 15-16",
  ],
  bolt: ["M13 2 4 14h7l-1 8 9-12h-7l1-8Z"],
  trophy: [
    "M8 4h8v6a4 4 0 1 1-8 0V4Z",
    "M8 6H5v1a3 3 0 0 0 3 3M16 6h3v1a3 3 0 0 1-3 3",
    "M12 14v4M9 21h6",
  ],
};

// Larger icons want a lighter stroke, or they read as heavy blocks.
export default function Icon({ name, className = "h-5 w-5", strokeWidth = 1.7 }) {
  const paths = PATHS[name];
  if (!paths) return null;

  return (
    <svg
      viewBox="0 0 24 24"
      className={className}
      fill="none"
      stroke="currentColor"
      strokeWidth={strokeWidth}
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      {paths.map((d) => (
        <path key={d} d={d} />
      ))}
    </svg>
  );
}
