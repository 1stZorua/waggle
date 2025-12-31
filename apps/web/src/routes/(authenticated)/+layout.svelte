<script lang="ts">
	import { Header, Sidebar } from '$components/layout';
	import { onDestroy, onMount, type Snippet } from 'svelte';
	import type { LayoutData } from './$types';
	import { startTokenRefresh, stopTokenRefresh } from '$lib/client/auth';
	import { page } from '$app/state';
	import { Modal } from '$components/shared/other';
	import { Post } from '$components/shared/posts';

	let refreshIntervalId: ReturnType<typeof setInterval>;

	let { children, data }: { children: Snippet; data: LayoutData } = $props();

	onMount(() => {
		refreshIntervalId = startTokenRefresh();
	});

	onDestroy(() => {
		stopTokenRefresh(refreshIntervalId);
	});
</script>

<div
	class="wrapper grid h-full w-full grid-cols-[20rem_1fr] grid-rows-[auto_1fr] [grid-template-areas:'header_header'_'aside_main'] max-xl:grid-cols-[4.75rem_1fr] max-md:grid-cols-[1fr] max-md:[grid-template-areas:'header'_'main']"
>
	<Header className="[grid-area:header]" {data}></Header>
	<Sidebar className="[grid-area:aside]" {data}></Sidebar>
	<main class="main-content w-full max-w-480 justify-self-center [grid-area:main]">
		{@render children?.()}
	</main>
</div>

{#if page.state.post}
	<Modal
		parentClassName="z-10 max-xl:hidden"
		className="shadow-none! bg-transparent h-full w-full pointer-events-none"
		isOpen={true}
		onClose={() => history.back()}
	>
		<div class="flex h-full w-full items-center justify-center p-8">
			<Post
				className="
					pointer-events-auto!
					w-full
					max-w-[75vw]
					aspect-4/5
					max-h-[90svh]
					overflow-y-auto
					portrait:aspect-auto
					portrait:max-h-200
				"
				post={page.state.post}
				variant="detail"
			/>
		</div>
	</Modal>
{/if}
