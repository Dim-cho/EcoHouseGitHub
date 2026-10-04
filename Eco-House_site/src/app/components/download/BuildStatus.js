const LINES = [
  ["build", "in development"],
  ["platforms", "windows · macos · linux"],
  ["price", "free during development"],
  ["release", "not announced", "text-[#f2c94c]"],
];

export default function BuildStatus() {
  return (
    <div
      className="mt-8 rounded-lg bg-ink p-5 font-mono text-[13px] leading-6 text-[#cfd6d1]"
      aria-label="Build status"
    >
      <p>
        <span className="text-[#7fcf9f]">$</span> eco-house --status
      </p>
      {LINES.map(([k, v, cls], i) => (
        <p key={k}>
          {k.padEnd(12, ".")} <span className={cls ?? "text-paper"}>{v}</span>
          {i === LINES.length - 1 && (
            <span className="caret ml-1 inline-block h-3.5 w-2 translate-y-0.5 bg-[#cfd6d1]" />
          )}
        </p>
      ))}
    </div>
  );
}
