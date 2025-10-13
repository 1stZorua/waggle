<script lang="ts">
	import type { Snippet } from 'svelte';
	import 'iconify-icon';
	import '../app.css';

	let { children }: { children?: Snippet } = $props();
</script>

{@render children?.()}
