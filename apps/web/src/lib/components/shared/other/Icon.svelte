<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	let { className, icon }: { className?: ClassType; icon: string } = $props();
</script>

<iconify-icon class={cn('text-xl inline-block w-[1em] h-[1em]', className)} {icon}></iconify-icon>
