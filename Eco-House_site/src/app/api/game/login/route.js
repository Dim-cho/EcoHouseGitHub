import { authenticate } from "@/lib/users";
import { createToken } from "@/lib/auth";

export const dynamic = "force-dynamic";

export async function POST(request) {
  let body;
  try {
    body = await request.json();
  } catch {
    return Response.json({ error: "Expected a JSON body." }, { status: 400 });
  }

  const username = String(body.username ?? "").trim();
  const password = String(body.password ?? "");

  if (!username || !password) {
    return Response.json({ error: "Enter your username and password." }, { status: 400 });
  }

  const result = await authenticate(username, password);
  if (result.error) return Response.json({ error: result.error }, { status: 401 });

  return Response.json({
    username: result.username,
    token: createToken(result.username),
  });
}
