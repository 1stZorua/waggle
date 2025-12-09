<script lang="ts">
	import { Header, Sidebar } from '$components/layout';
	import type { Snippet } from 'svelte';
	import type { LayoutData } from './$types';

	let { children, data }: { children: Snippet; data: LayoutData } = $props();
</script>

<div class="wrapper h-full w-full">
	<Header></Header>
	<div class="flex min-h-max">
		<Sidebar user={data.user}></Sidebar>
		<main class="main-content min-h-max flex-1">
			{@render children?.()}
		</main>
	</div>
</div>
