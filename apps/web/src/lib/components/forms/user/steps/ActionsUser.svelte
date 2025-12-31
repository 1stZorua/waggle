<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { Separator } from '$components/shared/other';
	import { TextBase } from '$components/shared/text';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import { page } from '$app/state';
	import { goto } from '$app/navigation';
	import type { User } from '@waggle/api-client/user/model';
	import { useUserModal } from '$lib/hooks/useUserModal.svelte';

	interface Props {
		data: LayoutData;
		profile: User | null;
		onClose: () => void;
	}

	let user = page.data.user;
	let { data, profile, onClose }: Props = $props();

	const isSelf = $derived(user?.id === profile?.id);

	function handleEdit() {
		goto(`/profile/settings`);
		onClose();
	}

	function handleDelete() {
		useUserModal.setMode('delete');
	}

	function handleReport() {
		onClose();
	}
</script>

<div class="gap-md flex h-auto w-150 flex-col">
	<div class="flex flex-col">
		{#if isSelf}
			<ButtonText onclick={handleDelete} className="justify-center p-4 w-full">
				<TextBase className="text-pink">Delete</TextBase>
			</ButtonText>
			<Separator></Separator>
			<ButtonText onclick={handleEdit} className="justify-center p-4 w-full">
				<TextBase>Edit</TextBase>
			</ButtonText>
		{:else}
			<ButtonText onclick={handleReport} className="justify-center p-4 w-full">
				<TextBase className="text-pink">Report</TextBase>
			</ButtonText>
		{/if}

		<Separator></Separator>
		<ButtonText onclick={onClose} className="justify-center p-4 w-full">
			<div class="gap-sm flex items-center">
				<TextBase>Cancel</TextBase>
			</div>
		</ButtonText>
	</div>
</div>
