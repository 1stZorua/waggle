<script lang="ts">
	import { Header, Sidebar } from '$components/layout';
	import { onDestroy, onMount, type Snippet } from 'svelte';
	import type { LayoutData } from './$types';
	import { startTokenRefresh, stopTokenRefresh } from '$lib/client/auth';

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
	<Header className="[grid-area:header]"></Header>
	<Sidebar className="[grid-area:aside]" {data}></Sidebar>
	<main class="main-content w-full max-w-480 justify-self-center [grid-area:main]">
		{@render children?.()}
	</main>
</div>
