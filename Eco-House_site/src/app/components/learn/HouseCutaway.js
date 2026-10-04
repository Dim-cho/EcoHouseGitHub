// Flat cutaway of a home with the three leak points numbered
// (1 standby, 2 lighting, 3 heat).

function Marker({ x, y, n }) {
  return (
    <g transform={`translate(${x} ${y})`}>
      <circle r="13" fill="#1f7a52" />
      <text
        y="4.5"
        fill="#fff"
        textAnchor="middle"
        fontSize="13"
        fontWeight="500"
        fontFamily="var(--font-geist-mono), ui-monospace, monospace"
      >
        {n}
      </text>
    </g>
  );
}

function Lamp({ x, cord }) {
  return (
    <g transform={`translate(${x} 151)`}>
      <path d={`M0 0 L0 ${cord}`} stroke="#1c211e" strokeWidth="1.5" />
      <path
        d={`M-16 ${cord} L16 ${cord} L10 ${cord + 14} L-10 ${cord + 14} Z`}
        fill="#1c211e"
      />
      <circle cx="0" cy={cord + 22} r="20" fill="#f6c945" opacity=".35" className="cut-glow" />
      <circle cx="0" cy={cord + 18} r="6" fill="#f6c945" />
    </g>
  );
}

export default function HouseCutaway() {
  return (
    <svg
      viewBox="0 0 520 420"
      className="h-auto w-full"
      role="img"
      aria-label="Cutaway of a house showing where energy leaks: standby power at the TV, lighting, and heat escaping through the roof and windows"
    >
      <path d="M90 150 L260 40 L430 150 Z" fill="#efe8d8" />
      <rect x="91" y="151" width="338" height="118" fill="#fbf9f4" />
      <rect x="91" y="271" width="338" height="108" fill="#fbf9f4" />

      <g fill="none" stroke="#1c211e" strokeWidth="2" strokeLinejoin="round" strokeLinecap="round">
        <path d="M60 170 L260 40 L460 170" />
        <path d="M90 150 L90 380 L430 380 L430 150" />
        <path d="M90 270 L430 270" strokeWidth="1.5" />
        <path d="M40 380 L480 380" />
      </g>

      <g className="cut-heat" fill="none" stroke="#d9772f" strokeWidth="2.5" strokeLinecap="round">
        <path d="M200 70 q-6 -9 0 -18 q6 -9 0 -18" />
        <path d="M260 50 q-6 -9 0 -18 q6 -9 0 -18" />
        <path d="M320 70 q-6 -9 0 -18 q6 -9 0 -18" />
        <path d="M440 320 q9 -6 18 0 q9 6 18 0" />
        <path d="M440 300 q9 -6 18 0 q9 6 18 0" />
      </g>

      <rect x="360" y="296" width="54" height="50" fill="#dcecf3" stroke="#1c211e" strokeWidth="1.5" />
      <path d="M387 296 L387 346 M360 321 L414 321" stroke="#1c211e" strokeWidth="1.2" />

      <g transform="translate(300 330)">
        <rect width="46" height="34" rx="3" fill="#fff" stroke="#1c211e" strokeWidth="1.5" />
        <path d="M9 4 L9 30 M18 4 L18 30 M27 4 L27 30 M36 4 L36 30" stroke="#1c211e" strokeWidth="1.2" />
      </g>

      <g transform="translate(120 300)">
        <rect width="96" height="58" rx="4" fill="#1c211e" />
        <rect x="5" y="5" width="86" height="48" rx="2" fill="#2b3640" />
        <rect x="40" y="58" width="16" height="10" fill="#1c211e" />
        <rect x="26" y="68" width="44" height="6" rx="3" fill="#1c211e" />
        <circle cx="86" cy="49" r="2.4" fill="#e0452f" className="cut-blink" />
      </g>

      <path d="M216 350 C236 352 240 366 254 366" fill="none" stroke="#1c211e" strokeWidth="1.5" />
      <rect x="252" y="356" width="16" height="20" rx="2" fill="#fff" stroke="#1c211e" strokeWidth="1.5" />

      <Lamp x={170} cord={40} />
      <Lamp x={340} cord={30} />

      <path
        d="M130 250 L130 228 Q130 220 138 220 L220 220 Q228 220 228 228 L228 250 Z"
        fill="#c9d9c9"
        stroke="#1c211e"
        strokeWidth="1.5"
      />

      <Marker x={262} y={330} n={1} />
      <Marker x={206} y={196} n={2} />
      <Marker x={372} y={64} n={3} />
    </svg>
  );
}
