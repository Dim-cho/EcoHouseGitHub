import { resetPlayerStats } from "@/lib/stats";
import { readBearer } from "@/lib/bearer";

export const dynamic = "force-dynamic";

// Destructive: wipes the player's progress. The client must send
// { "confirm": "reset" } so a stray request can't clear an account.
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

  if (body?.confirm !== "reset") {
    return Response.json(
      { error: 'Send {"confirm":"reset"} to start over.' },
      { status: 400 },
    );
  }

  const stats = await resetPlayerStats(session.username);
  return Response.json({ username: session.username, ...stats });
}
