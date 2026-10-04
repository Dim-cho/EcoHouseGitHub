import { readToken } from "@/lib/auth";

// Game clients send the session token as "Authorization: Bearer <token>"
// rather than as the cookie the website uses.
export function readBearer(request) {
  const header = request.headers.get("authorization") ?? "";
  const [scheme, token] = header.split(" ");
  if (scheme?.toLowerCase() !== "bearer" || !token) return null;
  return readToken(token);
}
