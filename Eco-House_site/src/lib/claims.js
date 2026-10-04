import { getDb } from "@/lib/mongodb";

export const ECO_POINTS_PER_POST = 10;

const COLLECTION = "ecoClaims";

export async function hasClaimed(username, slug) {
  const db = await getDb();
  const doc = await db.collection(COLLECTION).findOne({ username, slug });
  return Boolean(doc);
}

export async function claimedSlugs(username) {
  const db = await getDb();
  const docs = await db
    .collection(COLLECTION)
    .find({ username }, { projection: { _id: 0, slug: 1 } })
    .toArray();

  return docs.map((d) => d.slug);
}

// The unique index is what actually prevents double claims: two requests racing
// can both pass a findOne, but only one insert survives.
export async function claimPost(username, slug) {
  const db = await getDb();
  const col = db.collection(COLLECTION);
  await col.createIndex({ username: 1, slug: 1 }, { unique: true });

  try {
    await col.insertOne({ username, slug, claimedAt: new Date() });
  } catch (error) {
    if (error.code === 11000) return { error: "You've already claimed this one." };
    throw error;
  }

  await db
    .collection("stats")
    .updateOne(
      { username },
      { $inc: { ecoPoints: ECO_POINTS_PER_POST }, $setOnInsert: { username } },
      { upsert: true },
    );

  return { points: ECO_POINTS_PER_POST };
}
