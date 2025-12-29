<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { CardPrimary } from '$components/shared/cards';
	import { Icon, Logo } from '$components/shared/other';
	import { Post } from '$components/shared/posts';
	import { TextBase, TextSmall } from '$components/shared/text';
	import type { PageProps } from './$types';
	import { onMount } from 'svelte';

	let { data }: PageProps = $props();

	let posts = $state(data.pageInfo.items);
	let nextCursor = $state(data.pageInfo.nextCursor);
	let loadingMore = $state(false);
	let done = $state(false);

	function isSentinelVisible() {
		if (!sentinel) return false;
		const rect = sentinel.getBoundingClientRect();
		return rect.top < window.innerHeight;
	}

	async function loadMore() {
		if (!nextCursor || loadingMore || done) return;

		loadingMore = true;

		const res = await fetch('/api/posts', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ cursor: nextCursor })
		});

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

<section
	class="gap-md relative grid h-full grid-cols-[1fr_30rem] [grid-template-areas:'feed_action'] max-2xl:grid-cols-[1fr] max-2xl:[grid-template-areas:'feed']"
>
	<div class="w-full">
		<div></div>

		{#if posts.length > 0}
			<div class="shadow-ui mx-auto flex max-w-[620px] flex-col rounded-lg">
				{#each posts as post, index}
					<Post className="shadow-none!" {post} isLast={index < posts.length - 1}></Post>
				{/each}

				{#if loadingMore}
					<div class="py-4 text-center">
						<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
					</div>
				{/if}

				{#if done && !loadingMore}
					<div class="flex w-full items-center justify-center py-4">
						<Logo className="w-20 text-secondary"></Logo>
					</div>
				{/if}

				<div bind:this={sentinel}></div>
			</div>
		{:else}
			<p>No posts yet.</p>
		{/if}
	</div>

	<div class="action-bar gap-md fixed flex flex-col [grid-area:action] max-2xl:hidden">
		<CardPrimary className="h-full items-start">
			<div class="flex w-full items-center justify-between">
				<TextBase>Top Companions</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>
		</CardPrimary>
		<CardPrimary className="items-start h-full">
			<div class="flex w-full items-center justify-between">
				<TextBase>Buddies</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>
		</CardPrimary>
	</div>
</section>
