import { createUser, validateCredentials } from "@/lib/users";
import { grantStartingBalance } from "@/lib/stats";
import { createToken } from "@/lib/auth";

export const dynamic = "force-dynamic";

// The game client can't use Server Actions or cookies, so it gets the same
// session token as a bearer string instead.
export async function POST(request) {
  let body;
  try {
    body = await request.json();
  } catch {
    return Response.json({ error: "Expected a JSON body." }, { status: 400 });
  }

  const username = String(body.username ?? "").trim();
  const password = String(body.password ?? "");

  const invalid = validateCredentials(username, password);
  if (invalid) return Response.json({ error: invalid }, { status: 400 });

  const result = await createUser(username, password);
  if (result.error) return Response.json({ error: result.error }, { status: 409 });

  await grantStartingBalance(result.username);

  return Response.json({
    username: result.username,
    token: createToken(result.username),
  });
}
