<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	let { className, icon }: { className?: ClassType; icon: string } = $props();
</script>

<iconify-icon
	class={cn(
		'inline-block h-[1em] w-[1em] p-[0.125px] text-xl transition-colors duration-300',
		className
	)}
	{icon}
></iconify-icon>
