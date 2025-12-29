<script lang="ts">
	import { ButtonPrimary, ButtonSecondary, ButtonText } from '$components/shared/buttons/index.js';
	import { TextBase, TextSmall } from '$components/shared/text';
	import { Icon, Input, Logo, Separator } from '$components/shared/other';
	import { superForm } from 'sveltekit-superforms';

	let { data } = $props();

	const { form, errors, enhance } = superForm(data.form);
</script>

<section
	class="wrapper flex h-full min-h-screen items-center justify-center gap-28"
	aria-labelledby="login-heading"
>
	<div class="relative flex h-full w-[775px] max-2xl:hidden">
		<img
			class="absolute top-1/2 right-[54%] h-[350px] w-[320px] -translate-y-[47.5%] -rotate-12 rounded-xl object-cover"
			src="images/bird.webp"
			alt="bird"
		/>
		<img
			class="absolute top-1/2 left-1/2 z-10 h-[490px] w-[320px] -translate-x-1/2 -translate-y-1/2 rounded-xl object-cover"
			src="images/cat.webp"
			alt="cat"
		/>
		<img
			class="absolute top-1/2 left-[54%] h-[350px] w-[320px] -translate-y-[47.5%] rotate-12 rounded-xl object-cover"
			src="images/dog.webp"
			alt="dog"
		/>
	</div>
	<form
		action="?/login"
		method="POST"
		class="gap-lg max-sm:gap-md flex w-[450px] flex-col items-center"
		use:enhance
	>
		<Logo className="w-46 max-sm:w-[40%]"></Logo>
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
					name="identifier"
					type="text"
					required
					className="rounded-full flex-row-reverse"
					value={$form.identifier}
					errors={$errors.identifier}
					placeholder="Username or email address"
				>
					<Icon className="text-md text-secondary" icon="ic:outline-email"></Icon>
				</Input>
				<div class="gap-sm flex flex-col">
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
					<ButtonText type="button" className="place-self-end">
						<TextSmall className="text-blue-dark">Forgot Password?</TextSmall>
					</ButtonText>
				</div>
			</div>
		</div>
		<div class="gap-sm flex w-full flex-col items-center">
			<ButtonPrimary type="submit">
				<TextBase>Sign In</TextBase>
			</ButtonPrimary>
			<div class="gap-xs flex">
				<TextSmall className="text-secondary">Don't have an account?</TextSmall>
				<ButtonText href="/register" type="button">
					<TextSmall className="text-blue-dark">Create an account</TextSmall>
				</ButtonText>
			</div>
		</div>
	</form>
</section>
