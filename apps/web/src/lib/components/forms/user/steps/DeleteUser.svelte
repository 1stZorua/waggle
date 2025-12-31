<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Icon, Separator } from '$components/shared/other';
	import { TextBase, TextLarge } from '$components/shared/text';
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import type { User } from '@waggle/api-client/user/model';

	interface Props {
		data: LayoutData;
		profile: User | null;
		onClose: () => void;
	}

	let { data, profile, onClose }: Props = $props();

	const { form, enhance } = superForm(data.deleteUserForm, {
		onResult: ({ result }) => {
			if (result.type === 'success') onClose();
		}
	});

	$effect(() => {
		$form.id = profile?.id ?? '';
	});
</script>

<form class="gap-md flex h-auto w-150 flex-col" action="/users?/delete" method="POST" use:enhance>
	<input type="hidden" name="id" value={$form.id} />

	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Delete User</TextLarge>
		</div>
		<ButtonText type="button" onclick={onClose}>
			<Icon icon="material-symbols:close-rounded"></Icon>
		</ButtonText>
	</div>

	<Separator />

	<div class="gap-md flex flex-col">
		<TextBase className="text-secondary font-normal">
			Are you sure you want to delete your account? This action cannot be undone.
		</TextBase>

		<div class="gap-sm mt-2 flex justify-end">
			<ButtonAction type="button" className="rounded-lg" onclick={onClose}>Cancel</ButtonAction>
			<Button
				type="submit"
				props={{ variant: 'primary', size: 'md' }}
				className="bg-pink hover:bg-pink-light rounded-lg w-max border-none"
			>
				Delete
			</Button>
		</div>
	</div>
</form>
