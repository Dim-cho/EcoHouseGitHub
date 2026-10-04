"use server";

import { redirect } from "next/navigation";
import { authenticate, createUser, validateCredentials } from "@/lib/users";
import { clearSession, setSession } from "@/lib/session";
import { grantStartingBalance } from "@/lib/stats";

export async function signUpAction(_prev, formData) {
  const username = String(formData.get("username") ?? "").trim();
  const password = String(formData.get("password") ?? "");
  const confirm = String(formData.get("confirm") ?? "");

  const invalid = validateCredentials(username, password);
  if (invalid) return { error: invalid };
  if (password !== confirm) return { error: "Passwords do not match." };

  const result = await createUser(username, password);
  if (result.error) return { error: result.error };

  await grantStartingBalance(result.username);
  await setSession(result.username);
  redirect("/dashboard");
}

export async function logInAction(_prev, formData) {
  const username = String(formData.get("username") ?? "").trim();
  const password = String(formData.get("password") ?? "");

  if (!username || !password) {
    return { error: "Enter your username and password." };
  }

  const result = await authenticate(username, password);
  if (result.error) return { error: result.error };

  await setSession(result.username);
  redirect("/dashboard");
}

export async function logOutAction() {
  await clearSession();
  redirect("/");
}
