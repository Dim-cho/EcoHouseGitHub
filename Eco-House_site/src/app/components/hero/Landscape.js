// Hills, houses, trees, chimneys and turbines along the bottom of the hero.
// Pure presentation: every colour and transform comes from CSS vars that the
// parent's [data-state] drives.

function Turbine({ x, y, scale, delay }) {
  return (
    <g transform={`translate(${x} ${y}) scale(${scale})`}>
      <path d="M-1.6 0 L1.6 0 L.8 -42 L-.8 -42 Z" />
      <g transform="translate(0 -42)">
        <g className="ls-spin" style={{ animationDelay: delay }}>
          <path transform="rotate(0)" d="M0 0 L-2.6 -20 L0 -22 L2.6 -20 Z" />
          <path transform="rotate(120)" d="M0 0 L-2.6 -20 L0 -22 L2.6 -20 Z" />
          <path transform="rotate(240)" d="M0 0 L-2.6 -20 L0 -22 L2.6 -20 Z" />
        </g>
      </g>
    </g>
  );
}

function Chimney({ x, y }) {
  return (
    <g transform={`translate(${x} ${y})`}>
      <rect x="-5" y="-46" width="10" height="46" />
      <g className="ls-smoke">
        <circle cx="0" cy="-58" r="9" />
        <circle cx="-10" cy="-72" r="12" />
        <circle cx="-26" cy="-84" r="15" />
        <circle cx="-46" cy="-90" r="17" />
      </g>
    </g>
  );
}

function House({ x, y, scale }) {
  return (
    <g transform={`translate(${x} ${y}) scale(${scale})`}>
      <rect x="-14" y="-16" width="28" height="16" />
      <path d="M-18 -15 L0 -30 L18 -15 Z" />
    </g>
  );
}

function Tree({ x, y, scale }) {
  return (
    <g transform={`translate(${x} ${y})`}>
      <g className="ls-grow">
        <g transform={`scale(${scale})`}>
          <path d="M0 -30 L9 -4 L-9 -4 Z" />
          <rect x="-1.5" y="-5" width="3" height="5" />
        </g>
      </g>
    </g>
  );
}

export default function Landscape() {
  return (
    <svg
      className="absolute inset-x-0 bottom-0 h-[300px] w-full overflow-visible sm:h-[360px] lg:h-[400px]"
      viewBox="0 -180 1440 400"
      preserveAspectRatio="xMidYMax slice"
      aria-hidden="true"
    >
      <path
        className="ls-far"
        d="M0 150 C120 120 220 128 340 140 C470 152 560 108 700 112 C840 116 930 150 1060 138 C1190 126 1300 104 1440 118 L1440 220 L0 220 Z"
      />

      <g className="ls-smog-ink" style={{ opacity: "var(--smog)" }}>
        <Chimney x={1010} y={140} />
        <Chimney x={1040} y={138} />
        <rect x="985" y="118" width="80" height="22" />

        <Chimney x={240} y={136} />
      </g>

      {/* Clear of the factory at 985-1065 and the chimneys at 240/1010/1040 —
          both groups share this space, only one is visible at a time. */}
      <g className="ls-turb" style={{ opacity: "var(--green)" }}>
        <Turbine x={1120} y={132} scale={1.1} delay="0s" />
        <Turbine x={1290} y={128} scale={0.9} delay="-1.2s" />
        <Turbine x={1380} y={124} scale={0.75} delay="-2.1s" />
        <Turbine x={450} y={140} scale={0.9} delay="-.6s" />
        <Turbine x={290} y={134} scale={0.7} delay="-3.4s" />
      </g>

      <path
        className="ls-near"
        d="M0 186 C160 160 300 166 440 176 C600 188 720 160 880 164 C1040 168 1180 190 1440 172 L1440 220 L0 220 Z"
      />

      <g className="ls-house">
        <House x={560} y={176} scale={1} />
        <House x={610} y={172} scale={0.8} />
        <House x={880} y={166} scale={0.9} />
      </g>

      <g className="ls-tree">
        <Tree x={520} y={178} scale={1} />
        <Tree x={650} y={172} scale={0.8} />
        <Tree x={670} y={174} scale={0.6} />
        <Tree x={840} y={168} scale={0.9} />
        <Tree x={920} y={168} scale={0.7} />
        <Tree x={380} y={174} scale={0.8} />
        <Tree x={1220} y={182} scale={0.9} />
      </g>
    </svg>
  );
}
