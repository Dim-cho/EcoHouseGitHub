import { getDb } from "@/lib/mongodb";

// Never cached — this must reflect the live DB on every request.
export const dynamic = "force-dynamic";

export async function GET() {
  try {
    const db = await getDb();
    await db.command({ ping: 1 });
    return Response.json({ ok: true, db: db.databaseName });
  } catch (error) {
    return Response.json({ ok: false, error: error.message }, { status: 500 });
  }
}
