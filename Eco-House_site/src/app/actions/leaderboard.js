"use server";

import { findPlayerRank } from "@/lib/leaderboard";

export async function searchPlayerAction(_prev, formData) {
  const username = String(formData.get("username") ?? "").trim();
  if (!username) return null;

  return findPlayerRank(username);
}
