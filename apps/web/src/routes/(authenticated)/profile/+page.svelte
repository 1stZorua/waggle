<script lang="ts">
	import { ButtonAction, ButtonPrimary, ButtonText } from '$components/shared/buttons/index';
	import { Avatar, Ellipse, Icon, TabControl, InfiniteScroll } from '$components/shared/other';
	import { TextBase, TextLarge, TextSmall } from '$components/shared/text';
	import type { PageProps } from './$types';
	import { formatNumber } from '$lib/utils';
	import { goto, preloadData, pushState } from '$app/navigation';
	import type { Post } from '$lib/types/types';
	import { useUserModal } from '$lib/hooks/useUserModal.svelte';

	let { data }: PageProps = $props();

	let activeTabIndex = $state(0);

	let posts = $state(data.userPosts.items);
	let nextCursor = $state(data.userPosts.nextCursor);
	let loadingMore = $state(false);

	async function loadMore() {
		if (!nextCursor || loadingMore) return;

		loadingMore = true;

		const res = await fetch(`/api/posts/users/${data.user.id}?cursor=${nextCursor}`);
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
							<TextLarge>{data.user.name}</TextLarge>
							<ButtonText onclick={openUserModal} className="rounded-lg self-center" type="submit">
								<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
							</ButtonText>
						</div>
						<TextBase className="text-secondary">@{data.user.username}</TextBase>
					</div>
					<div class="gap-xs flex items-center">
						<div>
							<TextSmall>{data.profile?.postCount}</TextSmall>
							<TextSmall className="text-secondary font-normal">posts</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>{data.profile?.followerCount}</TextSmall>
							<TextSmall className="text-secondary font-normal">followers</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>{data.profile?.followingCount}</TextSmall>
							<TextSmall className="text-secondary font-normal">following</TextSmall>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="relative flex w-max items-center">
			<Avatar className="border-3 border-background bg-accent" src="images/dexter.webp" alt="pet"
			></Avatar>
			<ButtonPrimary
				className="flex items-center justify-center h-11 w-11 p-0 bg-pink rounded-full absolute left-2/3 border-3 border-pink-light ring-4 ring-background"
			>
				<Icon className="text-md text-background" icon="ic:round-plus"></Icon>
			</ButtonPrimary>
		</div>
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
</section>
