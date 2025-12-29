<script lang="ts">
	import { ButtonSecondary } from '$components/shared/buttons';
	import { TextBase } from '$components/shared/text';
	import { superForm } from 'sveltekit-superforms';
	import type { PageProps } from './$types';
	import { Input } from '$components/shared/other';

	let { data }: PageProps = $props();

	const { form, errors, enhance } = superForm({
		id: data.user.id
	});
</script>

<form action="?/delete" method="POST" use:enhance>
	<Input
		name="id"
		type="text"
		required
		className="rounded-full flex-row-reverse hidden"
		value={$form.id}
		errors={$errors.id}
		readonly
	></Input>
	<ButtonSecondary className="w-max" type="submit">
		<TextBase>Delete Account</TextBase>
	</ButtonSecondary>
</form>
