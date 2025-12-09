<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';

	let { className }: { className?: ClassType } = $props();
</script>

<span class={cn('bg-secondary h-1 w-1 rounded-full', className)}></span>
