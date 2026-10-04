import SiteNav from "@/app/components/SiteNav";
import SiteFooter from "@/app/components/SiteFooter";

// The dashboard sits outside the (site) group but wants the same chrome.
export default function DashboardLayout({ children }) {
  return (
    <>
      <SiteNav />
      {children}
      <SiteFooter />
    </>
  );
}
