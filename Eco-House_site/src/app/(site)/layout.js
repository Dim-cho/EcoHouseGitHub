import SiteFooter from "@/app/components/SiteFooter";

// No nav here: the landing page floats its own over the hero, and the other
// pages in this group need one in the normal flow.
export default function SiteLayout({ children }) {
  return (
    <>
      {children}
      <SiteFooter />
    </>
  );
}
