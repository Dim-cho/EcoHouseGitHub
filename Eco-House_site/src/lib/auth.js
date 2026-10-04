import { randomBytes, scrypt, timingSafeEqual, createHmac } from "node:crypto";
import { promisify } from "node:util";

const scryptAsync = promisify(scrypt);

const KEY_LEN = 64;
const SESSION_DAYS = 30;

export const SESSION_COOKIE = "eh_session";

function secret() {
  const s = process.env.SESSION_SECRET;
  if (!s || s.length < 32) {
    throw new Error("SESSION_SECRET must be set to at least 32 characters.");
  }
  return s;
}

// --- Passwords: scrypt with a per-user salt, stored as "salt:hash". ---

export async function hashPassword(password) {
  const salt = randomBytes(16).toString("hex");
  const derived = await scryptAsync(password, salt, KEY_LEN);
  return `${salt}:${derived.toString("hex")}`;
}

export async function verifyPassword(password, stored) {
  const [salt, hash] = String(stored).split(":");
  if (!salt || !hash) return false;

  const derived = await scryptAsync(password, salt, KEY_LEN);
  const expected = Buffer.from(hash, "hex");
  // Lengths must match before timingSafeEqual or it throws.
  if (expected.length !== derived.length) return false;
  return timingSafeEqual(expected, derived);
}

// --- Sessions: HMAC-signed token, no server-side session store needed. ---

function sign(payload) {
  return createHmac("sha256", secret()).update(payload).digest("base64url");
}

export function createToken(username) {
  const expires = Date.now() + SESSION_DAYS * 86_400_000;
  const payload = `${Buffer.from(username).toString("base64url")}.${expires}`;
  return `${payload}.${sign(payload)}`;
}

export function readToken(token) {
  if (!token) return null;

  const parts = String(token).split(".");
  if (parts.length !== 3) return null;

  const [user, expires, signature] = parts;
  const payload = `${user}.${expires}`;

  const expected = Buffer.from(sign(payload));
  const actual = Buffer.from(signature);
  if (expected.length !== actual.length) return null;
  if (!timingSafeEqual(expected, actual)) return null;

  if (Date.now() > Number(expires)) return null;

  return { username: Buffer.from(user, "base64url").toString("utf8") };
}

export const SESSION_MAX_AGE = SESSION_DAYS * 86_400;
