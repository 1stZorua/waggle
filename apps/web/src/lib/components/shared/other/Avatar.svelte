<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	let { className, src, alt }: { className?: ClassType; src: string; alt: string } = $props();
</script>

<img class={cn('h-12 w-12 shrink-0 rounded-full object-cover', className)} {src} {alt} />
