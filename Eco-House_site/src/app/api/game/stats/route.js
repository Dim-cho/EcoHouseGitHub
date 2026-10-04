import { getPlayerStats } from "@/lib/stats";
import { readBearer } from "@/lib/bearer";

export const dynamic = "force-dynamic";

export async function GET(request) {
  const session = readBearer(request);
  if (!session) {
    return Response.json({ error: "Sign in first." }, { status: 401 });
  }

  const stats = await getPlayerStats(session.username);
  return Response.json({ username: session.username, ...stats });
}
