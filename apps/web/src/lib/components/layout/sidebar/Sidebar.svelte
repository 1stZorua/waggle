<script lang="ts">
	import { TextBase, TextSmall } from '$components/shared/text';
	import { ButtonText } from '$components/shared/buttons';
	import { Card } from '$components/shared/cards';
	import { Avatar, Icon } from '$components/shared/other';
	import { Tag } from '$components/shared/tags';
	import { sidebarConfig } from './config';
	import { cn } from '$lib/utils/merge';
	import { page } from '$app/state';
	import { onNavigate } from '$app/navigation';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { AvatarForm, CommentForm, PostForm, UserForm } from '$components/forms';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';
	import type { ClassType } from '$components/_types';

	let currentPage = $derived(page.url.pathname);

	const items = sidebarConfig.map((item) => {
		if (item.title === 'Create') {
			return {
				...item,
				onClick: () => usePostModal.openCreate()
			};
		}
		return item;
	});

	onNavigate((navigation) => {
		if (!document.startViewTransition) return;
		return new Promise((resolve) => {
			document.startViewTransition(async () => {
				resolve();
				await navigation.complete;
			});
		});
	});

	let { className, data }: { className: ClassType; data: LayoutData } = $props();
</script>

<aside class={cn('sidebar fixed z-10 flex flex-col justify-between max-md:hidden', className)}>
	<div class="gap-md flex flex-col">
		<Card tag="a" href="/profile" className="items-center max-xl:hidden">
			<Avatar src={data.profile?.avatarUrl?.url ?? '/images/anonymous.png'} alt="Avatar" />
			<div class="flex flex-col">
				<TextBase>{data.user.name}</TextBase>
				<TextSmall className="text-secondary font-normal -mt-1 font-secondary"
					>@{data.user.username}</TextSmall
				>
			</div>
		</Card>
		<Card tag="nav" props={{ variant: 'primary' }} className="flex-col overflow-hidden">
			{#each items as item, index}
				{@const isActive =
					(index === 0 && currentPage === '/') || currentPage.includes(item.title.toLowerCase())}

				<ButtonText href={item.href} onclick={item.onClick} className="group gap-md p-2 w-full">
					{#if isActive}
						<span
							class="bg-primary absolute left-0 h-10 w-2 rounded-tr-full rounded-br-full [view-transition-name:header]"
						></span>
					{/if}
					<Icon
						className={cn('group-hover:text-primary', !isActive && 'text-secondary')}
						icon={item.icon}
					></Icon>
					<TextBase className={cn('max-xl:hidden', isActive && 'font-semibold')}
						>{item.title}</TextBase
					>
					{#if item.tag}
						<Tag className="m-auto max-xl:hidden" props={{ variant: item.tag.variant }}>
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
			<TextBase className="max-xl:hidden">Logout</TextBase>
		</ButtonText>
	</form>
</aside>

<PostForm {data}></PostForm>
<CommentForm {data}></CommentForm>
<UserForm {data}></UserForm>
<AvatarForm {data}></AvatarForm>
