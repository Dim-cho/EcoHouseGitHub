// Green serif-italic phrase with the hand-drawn underline. Use once per heading.
export default function Accent({ children }) {
  return (
    <span className="relative inline-block font-serif text-[1.08em] font-normal italic tracking-[-.01em] text-brand">
      {children}
      <svg
        className="absolute -bottom-1.5 left-0 h-2.5 w-full text-brand/40"
        viewBox="0 0 300 12"
        preserveAspectRatio="none"
        aria-hidden="true"
      >
        <path
          d="M2 8 C60 2 140 2 200 6 C240 8 270 7 298 4"
          fill="none"
          stroke="currentColor"
          strokeWidth="3"
          strokeLinecap="round"
        />
      </svg>
    </span>
  );
}
