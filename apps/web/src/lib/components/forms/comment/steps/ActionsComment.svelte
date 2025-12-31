<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { Separator } from '$components/shared/other';
	import { TextBase } from '$components/shared/text';
	import { useCommentModal } from '$lib/hooks/useCommentModal.svelte';
	import type { Comment } from '$lib/types/types';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import { page } from '$app/state';
	import { goto } from '$app/navigation';

	interface Props {
		data: LayoutData;
		comment: Comment | null;
		onClose: () => void;
	}

	let user = page.data.user;
	let { data, comment, onClose }: Props = $props();

	const isOwner = $derived(user?.id === comment?.userId);

	function handleEdit() {
		useCommentModal.setMode('edit');
	}

	function handleDelete() {
		useCommentModal.setMode('delete');
	}

	function handleViewProfile() {
		goto(`/users/${comment?.userId}`);
		onClose();
	}

	function handleReport() {
		onClose();
	}
</script>

<div class="gap-md flex h-auto w-150 flex-col">
	<div class="flex flex-col">
		{#if isOwner}
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
			<Separator></Separator>
			<ButtonText onclick={handleViewProfile} className="justify-center p-4 w-full">
				<TextBase>View profile</TextBase>
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
