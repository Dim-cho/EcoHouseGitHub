import { applySync } from "@/lib/stats";
import { readBearer } from "@/lib/bearer";

export const dynamic = "force-dynamic";

// Totals that only ever climb. The game sends its own figure and the server
// keeps the larger one, so a corrupted local save can't erase real progress.
const LIFETIME = ["kwhSaved", "housesCompleted", "playTimeMinutes", "upgradesOwned"];

// Balances are sent as deltas ("+15 earned, -200 spent") because the website
// also writes them — daily bonuses, blog rewards. Absolute totals would race.
const BALANCES = { coinsDelta: "coins", ecoPointsDelta: "ecoPoints" };

export async function POST(request) {
  const session = readBearer(request);
  if (!session) {
    return Response.json({ error: "Sign in first." }, { status: 401 });
  }

  let body;
  try {
    body = await request.json();
  } catch {
    return Response.json({ error: "Expected a JSON body." }, { status: 400 });
  }

  const increments = {};
  for (const [field, target] of Object.entries(BALANCES)) {
    const delta = toNumber(body[field]);
    if (delta !== 0) increments[target] = delta;
  }

  const maximums = {};
  for (const field of LIFETIME) {
    if (body[field] == null) continue;
    const value = toNumber(body[field]);
    if (value > 0) maximums[field] = value;
  }

  const stats = await applySync(session.username, { increments, maximums });
  return Response.json({ username: session.username, ...stats });
}

// Rejects NaN/Infinity so a malformed client can't write garbage into Mongo.
function toNumber(value) {
  const n = Number(value);
  return Number.isFinite(n) ? Math.trunc(n) : 0;
}
