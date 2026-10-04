"use server";

import { revalidatePath } from "next/cache";
import { getSession } from "@/lib/session";
import { claimBonus } from "@/lib/bonus";
import { addCoins } from "@/lib/stats";

const DAILY_COINS = 250;
const STREAK_BONUS = 50;

export async function claimDailyCoinsAction() {
  const session = await getSession();
  if (!session) return { error: "You must be logged in." };

  const result = await claimBonus(session.username);
  if (result.error) return result;

  // Longer streaks pay more, capped so a long absence isn't punishing to return from.
  const amount = DAILY_COINS + Math.min(result.streak - 1, 6) * STREAK_BONUS;
  await addCoins(session.username, amount, "daily");

  revalidatePath("/dashboard");
  return { streak: result.streak, amount };
}
