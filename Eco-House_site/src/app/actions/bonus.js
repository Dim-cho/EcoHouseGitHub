"use server";

import { revalidatePath } from "next/cache";
import { claimBonus } from "@/lib/bonus";
import { getSession } from "@/lib/session";

export async function claimBonusAction() {
  const session = await getSession();
  if (!session) return { error: "You must be logged in." };

  const result = await claimBonus(session.username);
  revalidatePath("/dashboard");
  return result;
}
