<script lang="ts">
	import { ButtonAction } from '$components/shared/buttons';
	import { TextBase, TextSmall } from '$components/shared/text';
	import { Avatar } from '$components/shared/other';
	import { CardPrimary } from '$components/shared/cards';
	import type { PageProps } from './$types';
	import { useAvatarModal } from '$lib/hooks/useAvatarModal.svelte';

	let { data }: PageProps = $props();

	function openAvatarModal(e: MouseEvent) {
		e.preventDefault();

		if (data.profile) {
			useAvatarModal.openActions(data.profile);
		}
	}
</script>

<section class="flex h-full w-full">
	<CardPrimary className="flex-col w-max">
		<div class="gap-xl flex w-full items-center justify-between">
			<div class="gap-sm flex items-center">
				<Avatar src={data.profile?.avatarUrl?.url ?? '/images/anonymous.png'} alt="Avatar" />
				<div class="flex flex-col">
					<TextBase>{data.user.name}</TextBase>
					<TextSmall className="text-secondary">@{data.user.username}</TextSmall>
				</div>
			</div>
			<ButtonAction className="rounded-lg" onclick={openAvatarModal}>
				<TextSmall>Change Photo</TextSmall>
			</ButtonAction>
		</div>
	</CardPrimary>
</section>
