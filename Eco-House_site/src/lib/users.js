import { getDb } from "@/lib/mongodb";
import { hashPassword, verifyPassword } from "@/lib/auth";

const COLLECTION = "users";

export const USERNAME_RE = /^[a-zA-Z0-9_]{3,20}$/;

export function validateCredentials(username, password) {
  if (!USERNAME_RE.test(username)) {
    return "Username must be 3-20 characters: letters, numbers, or underscores.";
  }
  if (typeof password !== "string" || password.length < 8) {
    return "Password must be at least 8 characters.";
  }
  if (password.length > 200) {
    return "Password is too long.";
  }
  return null;
}

async function users() {
  const db = await getDb();
  return db.collection(COLLECTION);
}

export async function createUser(username, password) {
  const col = await users();
  // Case-insensitive uniqueness: two players shouldn't be "Andrei" and "andrei".
  await col.createIndex({ usernameLower: 1 }, { unique: true });

  const doc = {
    username,
    usernameLower: username.toLowerCase(),
    passwordHash: await hashPassword(password),
    createdAt: new Date(),
  };

  try {
    await col.insertOne(doc);
  } catch (error) {
    if (error.code === 11000) return { error: "That username is taken." };
    throw error;
  }

  return { username };
}

export async function authenticate(username, password) {
  const col = await users();
  const user = await col.findOne({ usernameLower: String(username).toLowerCase() });

  // Same message either way so the form can't be used to enumerate usernames.
  const fail = { error: "Incorrect username or password." };
  if (!user) return fail;
  if (!(await verifyPassword(password, user.passwordHash))) return fail;

  return { username: user.username };
}

export async function findUser(username) {
  const col = await users();
  return col.findOne(
    { usernameLower: String(username).toLowerCase() },
    { projection: { _id: 0, passwordHash: 0 } },
  );
}
