<script lang="ts">
	import { ButtonAction, ButtonPrimary, ButtonText } from '$components/shared/buttons/index';
	import { Avatar, Ellipse, Icon, TabControl } from '$components/shared/other';
	import { TextBase, TextLarge, TextSmall } from '$components/shared/text';
	import { superForm } from 'sveltekit-superforms';
	import type { PageProps } from './$types';
	import { onMount } from 'svelte';

	let { data }: PageProps = $props();

	let activeTabIndex = $state(0);

	let posts = $state(data.userPosts.items);
	let nextCursor = $state(data.userPosts.nextCursor);
	let loadingMore = $state(false);
	let done = $state(false);

	function sleep(ms: number) {
		return new Promise((resolve) => setTimeout(resolve, ms));
	}

	function isSentinelVisible() {
		if (!sentinel) return false;
		const rect = sentinel.getBoundingClientRect();
		return rect.top < window.innerHeight;
	}

	async function loadMore() {
		if (!nextCursor || loadingMore || done) return;

		loadingMore = true;

		await sleep(1200);

		const res = await fetch(`/api/posts/users/${data.user.id}?cursor=${nextCursor}`);

		const result = await res.json();
		if (result.items.length === 0 || !result.nextCursor) done = true;

		posts = [...posts, ...result.items];
		nextCursor = result.nextCursor;
		loadingMore = false;

		requestAnimationFrame(() => {
			if (!done && isSentinelVisible()) loadMore();
		});
	}

	let observer: IntersectionObserver;
	let sentinel: HTMLDivElement | undefined = $state();

	onMount(() => {
		observer = new IntersectionObserver(
			(entries) => {
				if (entries[0].isIntersecting) {
					loadMore();
				}
			},
			{ rootMargin: '200px' }
		);

		if (sentinel) observer.observe(sentinel);

		return () => {
			if (observer && sentinel) observer.unobserve(sentinel);
		};
	});
</script>

<section class="gap-lg flex flex-col">
	<div class="gap-md flex w-full flex-col">
		<div class="flex justify-between">
			<div class="gap-md flex items-center">
				<Avatar className="w-30 h-30" src="images/avatar.png" alt="Avatar"></Avatar>
				<div class="gap-xs flex flex-col">
					<div class="flex flex-col">
						<TextLarge>{data.user.name}</TextLarge>
						<TextBase className="text-secondary">@{data.user.username}</TextBase>
					</div>
					<div class="gap-xs flex items-center">
						<div>
							<TextSmall>12</TextSmall>
							<TextSmall className="text-secondary font-normal">posts</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>257</TextSmall>
							<TextSmall className="text-secondary font-normal">followers</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<div>
							<TextSmall>16</TextSmall>
							<TextSmall className="text-secondary font-normal">following</TextSmall>
						</div>
					</div>
				</div>
			</div>
			<ButtonAction href="/profile/edit" className="rounded-lg" type="submit">
				<TextBase>Edit Profile</TextBase>
			</ButtonAction>
		</div>
		<div class="relative flex w-max items-center">
			<Avatar className="border-2 border-background bg-accent" src="images/dexter.webp" alt="pet"
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
		<!-- {#each ['All', 'Dexter'] as cat, index}
			{#if activeTabIndex === index}
				{cat}
			{/if}
		{/each} -->
		{#if activeTabIndex === 0}
			<div class="gap-sm grid grid-cols-4">
				{#each posts as post}
					<ButtonText
						className="group aspect-4/5 w-full relative overflow-hidden rounded-lg"
						href={`/posts/${post.id}`}
					>
						<img
							src={post.mediaUrls?.[post.thumbnailId]?.url}
							alt={post.caption}
							class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
						/>
						<div
							class="absolute h-full w-full bg-transparent transition-colors duration-500 group-hover:bg-black/15"
						></div>
						{#if post.mediaIds?.length ?? 0 > 0}
							<Icon
								icon="heroicons:square-2-stack-solid"
								className="absolute text-background top-4 right-4 z-10"
							></Icon>
						{/if}
					</ButtonText>
				{/each}
				{#if loadingMore}
					{#each Array(8) as _}
						<div class="bg-accent aspect-4/5 animate-pulse rounded-lg"></div>
					{/each}
				{/if}
			</div>

			<div bind:this={sentinel}></div>
		{/if}
	</div>
</section>
