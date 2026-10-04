import { getDb } from "@/lib/mongodb";

function dayKey(date = new Date()) {
  return date.toISOString().slice(0, 10);
}

export async function getBonusState(username) {
  const db = await getDb();
  const doc = await db
    .collection("bonuses")
    .findOne({ username }, { projection: { _id: 0 } });

  return {
    streak: doc?.streak ?? 0,
    lastClaim: doc?.lastClaim ?? null,
    claimedToday: doc?.lastClaim === dayKey(),
    // Reward value is deliberately unset until the Unity side defines it.
    pending: doc?.pending ?? 0,
  };
}

export async function claimBonus(username) {
  const db = await getDb();
  const today = dayKey();
  const col = db.collection("bonuses");

  const existing = await col.findOne({ username });
  if (existing?.lastClaim === today) {
    return { error: "Already claimed today. Come back tomorrow." };
  }

  const yesterday = dayKey(new Date(Date.now() - 86_400_000));
  const streak = existing?.lastClaim === yesterday ? (existing.streak ?? 0) + 1 : 1;

  await col.updateOne(
    { username },
    {
      $set: { username, lastClaim: today, streak },
      $inc: { pending: 1 },
    },
    { upsert: true },
  );

  return { streak };
}
