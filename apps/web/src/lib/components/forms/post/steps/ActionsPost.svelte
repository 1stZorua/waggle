<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { Separator } from '$components/shared/other';
	import { TextBase } from '$components/shared/text';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';
	import type { Post } from '$lib/types/types';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import { page } from '$app/state';
	import { goto } from '$app/navigation';

	interface Props {
		data: LayoutData;
		post: Post | null;
		onClose: () => void;
	}

	let user = page.data.user;
	let { data, post, onClose }: Props = $props();

	const isOwner = $derived(user?.id === post?.userId);

	function handleEdit() {
		usePostModal.setMode('edit');
	}

	function handleDelete() {
		usePostModal.setMode('delete');
	}

	function handleViewProfile() {
		goto(`/users/${post?.userId}`);
		onClose();
	}

	function handleViewPost() {
		goto(`/posts/${post?.id}`);
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
			<Separator></Separator>
			<ButtonText onclick={handleViewPost} className="justify-center p-4 w-full">
				<TextBase>View Post</TextBase>
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
