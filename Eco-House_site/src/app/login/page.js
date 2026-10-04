import AuthLayout from "@/app/components/auth/AuthLayout";
import AuthForm from "@/app/components/auth/AuthForm";
import { logInAction } from "@/app/actions/auth";

export const metadata = { title: "Log in · Eco-House" };

export default function LogInPage() {
  return (
    <AuthLayout>
      <AuthForm mode="login" action={logInAction} />
    </AuthLayout>
  );
}
