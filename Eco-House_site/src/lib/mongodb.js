import { MongoClient } from "mongodb";

// Connect lazily: reading env at module scope would break builds where it isn't set.
// Cached on globalThis so hot reloads and warm lambdas reuse one pool.
function getClientPromise() {
  const uri = process.env.MONGODB_URI;
  if (!uri) {
    throw new Error("Missing MONGODB_URI. Add it to .env.local (see .env.example).");
  }

  if (!globalThis._mongoClientPromise) {
    globalThis._mongoClientPromise = new MongoClient(uri).connect();
  }
  return globalThis._mongoClientPromise;
}

export async function getDb() {
  const client = await getClientPromise();
  return client.db(process.env.MONGODB_DB);
}
