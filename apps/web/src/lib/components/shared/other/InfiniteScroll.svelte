<script lang="ts">
	import type { Snippet } from 'svelte';

	interface Props {
		items: any[];
		hasMore: boolean;
		isLoading: boolean;
		onLoadMore: () => void | Promise<void>;
		rootMargin?: string;
		threshold?: number;
		scrollContainerSelector?: string;
		children?: Snippet<[{ items: any[] }]>;
		loadingContent?: Snippet;
	}

	let {
		items,
		hasMore,
		isLoading = false,
		onLoadMore,
		rootMargin = '200px',
		threshold = 0.01,
		scrollContainerSelector = '[data-infinite-scroll]',
		children,
		loadingContent
	}: Props = $props();

	let sentinel: HTMLDivElement | undefined = $state();
	let observer: IntersectionObserver | undefined;

	function checkIfSentinelVisible() {
		if (!sentinel) return false;

		const scrollContainer = sentinel.closest(scrollContainerSelector);

		if (scrollContainer) {
			const containerRect = scrollContainer.getBoundingClientRect();
			const sentinelRect = sentinel.getBoundingClientRect();
			return sentinelRect.top < containerRect.bottom && sentinelRect.bottom > containerRect.top;
		}

		const rect = sentinel.getBoundingClientRect();
		return rect.top < window.innerHeight && rect.bottom > 0;
	}

	function setupObserver() {
		if (!sentinel || !hasMore || isLoading) return;

		if (observer) {
			observer.disconnect();
			observer = undefined;
		}

		const scrollContainer = sentinel.closest(scrollContainerSelector);

		observer = new IntersectionObserver(
			(entries) => {
				entries.forEach((entry) => {
					if (entry.isIntersecting && !isLoading && hasMore) {
						onLoadMore();
					}
				});
			},
			{
				root: scrollContainer as Element | null,
				rootMargin,
				threshold
			}
		);

		observer.observe(sentinel);

		setTimeout(() => {
			if (checkIfSentinelVisible() && !isLoading && hasMore) {
				onLoadMore();
			}
		}, 50);
	}

	$effect(() => {
		if (sentinel && hasMore && !isLoading) setupObserver();

		return () => {
			if (observer) {
				observer.disconnect();
				observer = undefined;
			}
		};
	});

	$effect(() => {
		const _ = items.length;

		if (!isLoading && hasMore && sentinel) {
			setTimeout(() => {
				if (checkIfSentinelVisible() && !isLoading && hasMore) {
					onLoadMore();
				}
			}, 50);
		}
	});
</script>

{@render children?.({ items })}

{#if hasMore}
	{#if isLoading}
		{@render loadingContent?.()}
	{/if}
	<div bind:this={sentinel} class="h-px w-full shrink-0"></div>
{/if}
