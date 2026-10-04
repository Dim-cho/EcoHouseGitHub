import AuthLayout from "@/app/components/auth/AuthLayout";
import AuthForm from "@/app/components/auth/AuthForm";
import { signUpAction } from "@/app/actions/auth";

export const metadata = { title: "Sign up · Eco-House" };

export default function SignUpPage() {
  return (
    <AuthLayout>
      <AuthForm mode="signup" action={signUpAction} />
    </AuthLayout>
  );
}
