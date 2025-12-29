<script lang="ts">
	import { ButtonPrimary, ButtonSecondary, ButtonText } from '$components/shared/buttons/index.js';
	import { TextBase, TextSmall } from '$components/shared/text';
	import { Icon, Input, Separator } from '$components/shared/other';
	import { superForm } from 'sveltekit-superforms';

	let { data } = $props();

	const { form, errors, enhance } = superForm(data.form);
</script>

<section
	class="wrapper flex h-full min-h-screen items-center justify-center gap-28"
	aria-labelledby="register-heading"
>
	<form
		action="?/register"
		method="POST"
		class="gap-lg max-sm:gap-md flex w-[450px] flex-col items-center"
		use:enhance
	>
		<img class="w-46 max-sm:w-[40%]" src="images/logo.svg" alt="Waggle" />
		<div class="gap-md max-sm:gap-sm flex w-full flex-col items-center">
			<div class="gap-sm flex w-full">
				<ButtonSecondary type="button" className="flex items-center gap-xs flex-1 px-0">
					<Icon className="text-md" icon="flat-color-icons:google"></Icon>
					<TextBase>Google</TextBase>
				</ButtonSecondary>
				<ButtonSecondary type="button" className="flex items-center gap-xs flex-1 px-0">
					<Icon className="text-md" icon="logos:facebook"></Icon>
					<TextBase>Facebook</TextBase>
				</ButtonSecondary>
			</div>
			<div class="w-full">
				<Separator>
					<TextSmall className="text-secondary">or continue with credentials</TextSmall>
				</Separator>
			</div>
			<div class="gap-md max-sm:gap-sm flex w-full flex-col">
				<Input
					name="username"
					type="text"
					required
					className="rounded-full flex-row-reverse"
					value={$form.username}
					errors={$errors.username}
					placeholder="Username"
				>
					<Icon className="text-md text-secondary" icon="proicons:tag"></Icon>
				</Input>
				<Input
					name="email"
					type="text"
					required
					className="rounded-full flex-row-reverse"
					value={$form.email}
					errors={$errors.email}
					placeholder="Email address"
				>
					<Icon className="text-md text-secondary" icon="ic:outline-email"></Icon>
				</Input>
				<div class="gap-md max-sm:gap-sm flex max-md:flex-col">
					<Input
						name="firstName"
						type="text"
						required
						className="rounded-full flex-row-reverse"
						value={$form.firstName}
						errors={$errors.firstName}
						placeholder="First name"
					>
						<Icon className="text-md text-secondary" icon="bx:user"></Icon>
					</Input>
					<Input
						name="lastName"
						type="text"
						required
						className="rounded-full flex-row-reverse"
						value={$form.lastName}
						errors={$errors.lastName}
						placeholder="Last name"
					>
						<Icon className="text-md text-secondary" icon="bx:user"></Icon>
					</Input>
				</div>
				<Input
					name="password"
					type="password"
					required
					className="rounded-full flex-row-reverse"
					value={$form.password}
					errors={$errors.password}
					placeholder="Password"
				>
					<Icon className="text-md text-secondary" icon="material-symbols:lock-outline"></Icon>
				</Input>
				<Input
					name="confirmPassword"
					type="password"
					required
					className="rounded-full flex-row-reverse"
					value={$form.confirmPassword}
					errors={$errors.confirmPassword}
					placeholder="Confirm password"
				>
					<Icon className="text-md text-secondary" icon="material-symbols:lock-outline"></Icon>
				</Input>
			</div>
		</div>
		<div class="gap-sm flex w-full flex-col items-center">
			<ButtonPrimary type="submit">
				<TextBase>Sign Up</TextBase>
			</ButtonPrimary>
			<div class="gap-xs flex">
				<TextSmall className="text-secondary">Already have an account?</TextSmall>
				<ButtonText href="/login" type="button">
					<TextSmall className="text-blue-dark">Login</TextSmall>
				</ButtonText>
			</div>
		</div>
	</form>
</section>
