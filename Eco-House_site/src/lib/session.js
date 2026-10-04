import { cookies } from "next/headers";
import { SESSION_COOKIE, SESSION_MAX_AGE, createToken, readToken } from "@/lib/auth";

export async function setSession(username) {
  const store = await cookies();
  store.set(SESSION_COOKIE, createToken(username), {
    httpOnly: true,
    sameSite: "lax",
    secure: process.env.NODE_ENV === "production",
    maxAge: SESSION_MAX_AGE,
    path: "/",
  });
}

export async function clearSession() {
  const store = await cookies();
  store.delete(SESSION_COOKIE);
}

export async function getSession() {
  const store = await cookies();
  return readToken(store.get(SESSION_COOKIE)?.value);
}
