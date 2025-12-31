<script lang="ts">
	import { onMount } from 'svelte';
	import { ButtonAction, ButtonPrimary, ButtonText } from '$components/shared/buttons/index';
	import { Avatar, Ellipse, Icon, TabControl, InfiniteScroll } from '$components/shared/other';
	import { TextBase, TextLarge, TextSmall } from '$components/shared/text';
	import type { PageProps } from './$types';
	import { formatNumber } from '$lib/utils';
	import { goto, preloadData, pushState } from '$app/navigation';
	import type { Post } from '$lib/types/types';
	import { useToggleFollow } from '$lib/hooks/useToggleFollow.svelte';
	import { useUserModal } from '$lib/hooks/useUserModal.svelte';

	let { data, params }: PageProps = $props();

	let isOwnProfile = $derived(data.user.id === data.profile?.id);

	let follow = useToggleFollow(params.id, data.profile?.followerCount ?? 0);

	let activeTabIndex = $state(0);

	let posts = $state(data.userPosts.items);
	let nextCursor = $state(data.userPosts.nextCursor);
	let loadingMore = $state(false);

	onMount(() => {
		if (!data.profile?.id) return;
		follow.init();
	});

	async function loadMore() {
		if (!nextCursor || loadingMore) return;

		loadingMore = true;

		const res = await fetch(`/api/posts/users/${data.profile?.id}?cursor=${nextCursor}`);
		const result = await res.json();

		posts = [...posts, ...result.items];
		nextCursor = result.nextCursor;
		loadingMore = false;
	}

	async function openViewPostModal(e: MouseEvent) {
		const isSmallScreen = window.matchMedia('(max-width:1280px)').matches;
		if (isSmallScreen) return;
		e.preventDefault();

		const { href } = e.currentTarget as HTMLAnchorElement;
		const result = await preloadData(href);

		if (result.type === 'loaded' && result.status === 200) {
			pushState(href, { post: (await result.data['post']) as Post });
		} else {
			goto(href);
		}
	}

	function openUserModal(e: MouseEvent) {
		e.preventDefault();

		if (data.profile) {
			useUserModal.openActions(data.profile);
		}
	}
</script>

<section class="gap-lg flex flex-col">
	<div class="gap-md flex w-full flex-col">
		<div class="flex justify-between">
			<div class="gap-md flex w-full items-center">
				<Avatar
					className="w-30 h-30"
					src={data.profile?.avatarUrl?.url ?? '/images/anonymous.png'}
					alt="Avatar"
				></Avatar>
				<div class="gap-xs flex flex-1 flex-col">
					<div class="flex flex-col">
						<div class="flex justify-between">
							<TextLarge>{data.profile?.firstName} {data.profile?.lastName}</TextLarge>
							{#if isOwnProfile}
								<ButtonText
									onclick={openUserModal}
									className="rounded-lg self-center"
									type="submit"
								>
									<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
								</ButtonText>
							{/if}
						</div>
						<TextBase className="text-secondary">@{data.profile?.username}</TextBase>
					</div>
					<div class="gap-xs flex items-center">
						<div>
							<TextSmall>{formatNumber(data.profile?.postCount)}</TextSmall>
							<TextSmall className="text-secondary font-normal">posts</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>{formatNumber(follow.count)}</TextSmall>
							<TextSmall className="text-secondary font-normal">followers</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>{formatNumber(data.profile?.followingCount)}</TextSmall>
							<TextSmall className="text-secondary font-normal">following</TextSmall>
						</div>
					</div>
				</div>
			</div>

			{#if !isOwnProfile}
				{#if follow.active}
					<ButtonAction className="rounded-lg" onclick={follow.toggle}>
						<TextBase>Following</TextBase>
					</ButtonAction>
				{:else}
					<ButtonAction className="rounded-lg" onclick={follow.toggle}>
						<TextBase>Follow</TextBase>
					</ButtonAction>
				{/if}
			{/if}
		</div>

		<div class="relative flex w-max items-center">
			<Avatar className="border-3 border-background bg-accent" src="/images/dexter.webp" alt="pet 1"
			></Avatar>
			<Avatar
				className="border-3 border-background bg-accent absolute left-1/2"
				src="/images/cat.webp"
				alt="pet 2"
			></Avatar>
			<Avatar
				className="border-3 border-background bg-accent absolute left-full"
				src="/images/dexter.webp"
				alt="pet 3"
			></Avatar>
		</div>
		<div class="gap-md flex flex-col">
			<TabControl
				items={['All', 'Dexter']}
				changeTab={(activeIndex) => (activeTabIndex = activeIndex)}
			></TabControl>
			{#if activeTabIndex === 0}
				<InfiniteScroll
					items={posts}
					hasMore={!!nextCursor}
					isLoading={loadingMore}
					onLoadMore={loadMore}
				>
					{#snippet children({ items })}
						<div class="gap-sm grid grid-cols-4">
							{#each items as post}
								<ButtonText
									className="group aspect-4/5 w-full relative overflow-hidden rounded-lg"
									href={`/posts/${post.id}`}
									onclick={openViewPostModal}
								>
									<img
										src={post.mediaUrls?.[post.thumbnailId]?.url}
										alt={post.caption}
										class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
									/>
									<div
										class="group text-background gap-sm absolute flex h-full w-full items-center justify-center bg-transparent transition-colors duration-500 group-hover:bg-black/25"
									>
										<div
											class="flex items-center gap-1 opacity-0 transition-opacity duration-500 group-hover:opacity-100"
										>
											<Icon icon="iconoir:heart-solid"></Icon>
											<TextSmall>{formatNumber(post?.likeCount)}</TextSmall>
										</div>
										<div
											class="flex items-center gap-1 opacity-0 transition-opacity duration-500 group-hover:opacity-100"
										>
											<Icon icon="iconamoon:comment-fill"></Icon>
											<TextSmall>{formatNumber(post?.commentCount)}</TextSmall>
										</div>
									</div>
									{#if post.mediaIds?.length ?? 0 > 0}
										<Icon
											icon="heroicons:square-2-stack-solid"
											className="absolute text-background top-4 right-4 z-10"
										></Icon>
									{/if}
								</ButtonText>
							{/each}
						</div>
					{/snippet}

					{#snippet loadingContent()}
						<div class="gap-sm grid grid-cols-4">
							{#each Array(8) as _}
								<div class="bg-accent aspect-4/5 animate-pulse rounded-lg"></div>
							{/each}
						</div>
					{/snippet}
				</InfiniteScroll>
			{/if}
		</div>
	</div>
</section>
