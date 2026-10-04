import { Geist, Geist_Mono } from "next/font/google";
import localFont from "next/font/local";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

const instrumentSerif = localFont({
  src: "../../public/fonts/InstrumentSerif-Italic.woff2",
  weight: "400",
  style: "italic",
  variable: "--font-instrument-serif",
  display: "swap",
});

// The Unity game's own typeface (MatrixType Display Bold, CC0) — used only for
// the "game layer": scores, ranks, status labels.
const matrix = localFont({
  src: "../../public/fonts/MatrixTypeDisplay-Bold.woff2",
  weight: "700",
  variable: "--font-matrix",
  display: "swap",
});

export const metadata = {
  title: "Eco-House: upgrade the house, clear the sky",
  description:
    "An idle game about cutting a home's energy bill to nothing. Swap out old appliances, bank every watt, and watch the world outside get cleaner.",
};

export default function RootLayout({ children }) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} ${instrumentSerif.variable} ${matrix.variable} h-full antialiased`}
    >
      <body className="flex min-h-full flex-col">{children}</body>
    </html>
  );
}
