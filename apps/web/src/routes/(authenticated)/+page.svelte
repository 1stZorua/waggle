<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { CardPrimary } from '$components/shared/cards';
	import { Icon, Logo, InfiniteScroll } from '$components/shared/other';
	import { Post } from '$components/shared/posts';
	import { TextBase, TextSmall } from '$components/shared/text';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();

	let posts = $state(data.pageInfo.items);
	let nextCursor = $state(data.pageInfo.nextCursor);
	let loadingMore = $state(false);

	async function loadMore() {
		if (!nextCursor || loadingMore) return;

		loadingMore = true;

		try {
			const res = await fetch('/api/posts', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ cursor: nextCursor })
			});

			const result = await res.json();

			posts = [...posts, ...result.items];
			nextCursor = result.nextCursor;
		} finally {
			loadingMore = false;
		}
	}
</script>

<section
	class="gap-md relative grid h-full grid-cols-[1fr_30rem] [grid-template-areas:'feed_action'] max-2xl:grid-cols-[1fr] max-2xl:[grid-template-areas:'feed']"
>
	<div class="w-full">
		<div></div>

		{#if posts.length > 0}
			<div class="shadow-ui mx-auto flex max-w-[620px] flex-col rounded-lg">
				<InfiniteScroll
					items={posts}
					hasMore={!!nextCursor}
					isLoading={loadingMore}
					onLoadMore={loadMore}
				>
					{#snippet children({ items })}
						{#each items as post, index}
							<Post className="shadow-none!" {post} isLast={index < items.length - 1}></Post>
						{/each}
					{/snippet}

					{#snippet loadingContent()}
						<div class="py-4 text-center">
							<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
						</div>
					{/snippet}
				</InfiniteScroll>

				{#if !nextCursor && !loadingMore}
					<div class="flex w-full items-center justify-center py-4">
						<Logo className="w-20 text-secondary"></Logo>
					</div>
				{/if}
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
