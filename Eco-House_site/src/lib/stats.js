import { getDb } from "@/lib/mongodb";

// Stats are written by the game; the site only reads them. A player who has
// never synced gets zeroes rather than an error.
const EMPTY = {
  kwhSaved: 0,
  coins: 0,
  ecoPoints: 0,
  housesCompleted: 0,
  upgradesOwned: 0,
  tier: "Starter",
  playTimeMinutes: 0,
  lastSync: null,
};

export async function getPlayerStats(username) {
  const db = await getDb();
  const doc = await db
    .collection("stats")
    .findOne({ username }, { projection: { _id: 0, username: 0 } });

  return { ...EMPTY, ...doc };
}

export async function getPlayerRank(username) {
  const db = await getDb();
  const me = await db.collection("stats").findOne({ username });
  if (!me?.kwhSaved) return null;

  const ahead = await db
    .collection("stats")
    .countDocuments({ kwhSaved: { $gt: me.kwhSaved } });

  return ahead + 1;
}

// Enough to afford the first upgrade or two, so a new player has something to
// spend before they've earned anything.
export const STARTING_COINS = 250;

export async function grantStartingBalance(username) {
  const db = await getDb();
  // upsert + $setOnInsert: a returning player's balance is never overwritten.
  await db
    .collection("stats")
    .updateOne(
      { username },
      { $setOnInsert: { username, coins: STARTING_COINS } },
      { upsert: true },
    );
}

// Starting over clears the run — balances and the house. Lifetime totals stay:
// kwhSaved is what the leaderboard ranks, and it's a record of energy the player
// really did save, not a score to be taken back.
export async function resetPlayerStats(username) {
  const db = await getDb();
  const doc = await db.collection("stats").findOneAndUpdate(
    { username },
    {
      $set: {
        coins: 0,
        ecoPoints: 0,
        upgradesOwned: 0,
        lastReset: new Date(),
      },
      $setOnInsert: { username },
    },
    { upsert: true, returnDocument: "after", projection: { _id: 0, username: 0 } },
  );

  return { ...EMPTY, ...doc };
}

// One atomic update so a sync can't interleave with a website reward and lose
// one of the two writes.
export async function applySync(username, { increments, maximums }) {
  const db = await getDb();
  const update = {
    $setOnInsert: { username },
    $set: { lastSync: new Date() },
  };

  if (Object.keys(increments).length) update.$inc = increments;
  if (Object.keys(maximums).length) update.$max = maximums;

  const doc = await db.collection("stats").findOneAndUpdate({ username }, update, {
    upsert: true,
    returnDocument: "after",
    projection: { _id: 0, username: 0 },
  });

  return { ...EMPTY, ...doc };
}

export async function addCoins(username, amount, reason) {
  const db = await getDb();
  await db.collection("stats").updateOne(
    { username },
    {
      $inc: { coins: amount },
      $setOnInsert: { username },
      $set: { lastReward: { amount, reason, at: new Date() } },
    },
    { upsert: true },
  );
}
