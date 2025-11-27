<script lang="ts">
	import { ButtonPrimary, ButtonSecondary, ButtonText } from '$components/shared/buttons/index.js';
	import { TextBase, TextSmall } from '$components/shared/text';
	import { Icon, Input, Separator } from '$components/shared/other';
	import { superForm } from 'sveltekit-superforms';

	let { data } = $props();

	const { form, message, errors, enhance } = superForm(data.form);
</script>

<!-- <form action="?/login" method="POST" use:enhance>
	<input required type="text" name="username" value={$form.username} placeholder="Username" />
	<input required type="password" name="password" value={$form.password} placeholder="Password" />
	<button type="submit">Login</button>
</form> -->

{#if $message}
	<span class="text-pink absolute z-10">{$message}</span>
{/if}

<section class="flex h-full items-center justify-center gap-40" aria-labelledby="login-heading">
	<div class="relative flex h-full w-[800px]">
		<img
			class="absolute top-1/2 right-[55%] h-[375px] w-[325px] -translate-y-[47.5%] -rotate-12 rounded-xl object-cover"
			src="images/bird.webp"
			alt="bird"
		/>
		<img
			class="absolute top-1/2 left-1/2 z-10 h-[515px] w-[325px] -translate-x-1/2 -translate-y-1/2 rounded-xl object-cover"
			src="images/cat.webp"
			alt="cat"
		/>
		<img
			class="absolute top-1/2 left-[55%] h-[375px] w-[325px] -translate-y-[47.5%] rotate-12 rounded-xl object-cover"
			src="images/dog.webp"
			alt="dog"
		/>
	</div>
	<form
		action="?/login"
		method="POST"
		class="gap-lg flex w-[450px] flex-col items-center"
		use:enhance
	>
		<img class="w-52" src="images/logo.svg" alt="Waggle" />
		{#if $message}
			<span class="text-pink absolute z-10">{$message}</span>
		{/if}
		<div class="gap-md flex w-full flex-col items-center">
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
			<div class="gap-md flex w-full flex-col">
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
			<ButtonPrimary type="submit" className="border-8 border-pink-light">
				<TextBase>Sign In</TextBase>
			</ButtonPrimary>
			<div class="gap-xs flex">
				<TextSmall className="text-secondary">Don't have an account?</TextSmall>
				<ButtonText type="button">
					<TextSmall className="text-blue-dark">Create an account</TextSmall>
				</ButtonText>
			</div>
		</div>
	</form>
</section>
