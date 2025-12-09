<script lang="ts">
	import { TextBase, TextSmall } from '$components/shared/text';
	import { ButtonText } from '$components/shared/buttons';
	import { Card, CardPrimary } from '$components/shared/cards';
	import { Avatar, Icon } from '$components/shared/other';
	import { Tag } from '$components/shared/tags';
	import type { User } from '$lib/types/types';
	import { sidebarConfig } from './config';
	import { cn } from '$lib/utils/merge';
	import { page } from '$app/state';
	import { onNavigate } from '$app/navigation';

	let currentPage = $derived(page.url.pathname);

	onNavigate((navigation) => {
		if (!document.startViewTransition) return;
		return new Promise((resolve) => {
			document.startViewTransition(async () => {
				resolve();
				await navigation.complete;
			});
		});
	});

	let { user }: { user: User } = $props();
</script>

<aside class="sidebar gap-md fixed flex flex-col justify-between">
	<div class="gap-md flex flex-col">
		<CardPrimary className="items-center">
			<Avatar src="images/avatar.png" alt="Avatar" />
			<div class="flex flex-col">
				<TextBase>{user.name}</TextBase>
				<TextSmall className="text-secondary font-normal -mt-1">@{user.username}</TextSmall>
			</div>
		</CardPrimary>
		<Card tag="nav" props={{ variant: 'primary' }} className="flex-col overflow-hidden">
			{#each sidebarConfig as item, index}
				{@const isActive =
					(index === 0 && currentPage === '/') || currentPage.includes(item.title.toLowerCase())}

				<ButtonText href={item.href} className="group gap-md p-2 w-full">
					{#if isActive}
						<span
							class="bg-primary absolute left-0 h-10 w-2 rounded-tr-full rounded-br-full [view-transition-name:header]"
						></span>
					{/if}
					<Icon
						className={cn('group-hover:text-primary', !isActive && 'text-secondary')}
						icon={item.icon}
					></Icon>
					<TextBase className={cn(isActive && 'font-semibold')}>{item.title}</TextBase>
					{#if item.tag}
						<Tag className="m-auto" props={{ variant: item.tag.variant }}>
							<TextSmall className="text-xs">{item.tag.label}</TextSmall>
						</Tag>
					{/if}
				</ButtonText>
			{/each}
		</Card>
	</div>
	<form action="/logout" method="POST">
		<ButtonText className="text-pink gap-lg p-6 hover:text-pink-light w-full" type="submit">
			<Icon icon="material-symbols:logout-rounded"></Icon>
			<TextBase>Logout</TextBase>
		</ButtonText>
	</form>
</aside>
