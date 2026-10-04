"use server";

import { revalidatePath } from "next/cache";
import { getSession } from "@/lib/session";
import { claimPost } from "@/lib/claims";
import { getArticle } from "@/lib/articles";

export async function claimEcoPointsAction(_prev, formData) {
  const session = await getSession();
  if (!session) return { error: "Log in to collect eco points." };

  const slug = String(formData.get("slug") ?? "");
  // Checked against the article list so a crafted form can't mint points for
  // a slug that doesn't exist.
  if (!getArticle(slug)) return { error: "Unknown guide." };

  const result = await claimPost(session.username, slug);
  if (result.error) return result;

  revalidatePath("/dashboard");
  return { points: result.points };
}
