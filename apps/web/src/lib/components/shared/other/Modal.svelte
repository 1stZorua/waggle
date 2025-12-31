<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';

	interface Props {
		parentClassName?: ClassType;
		className?: ClassType;
		children?: Snippet;
		isOpen: boolean;
		onClose?: () => void;
	}

	let backdrop: HTMLDivElement | undefined = $state();
	let { parentClassName, className, children, isOpen = $bindable(), onClose }: Props = $props();

	function handleClose() {
		isOpen = false;
		onClose?.();
	}

	$effect(() => {
		if (isOpen) {
			const originalHtmlOverflow = document.documentElement.style.overflow;

			document.documentElement.style.overflow = 'hidden';

			return () => {
				document.documentElement.style.overflow = originalHtmlOverflow;
			};
		}
	});
</script>

{#if isOpen}
	<div
		role="presentation"
		bind:this={backdrop}
		class={cn(
			'fixed inset-0 z-50 flex h-full w-full max-w-full items-center justify-center bg-black/25',
			parentClassName
		)}
		onclick={(e: MouseEvent) => e.target === backdrop && handleClose()}
		onkeydown={(e: KeyboardEvent) => {}}
	>
		<div class={cn('bg-background wrapper shadow-ui min-h-max min-w-max rounded-xl', className)}>
			{@render children?.()}
		</div>
	</div>
{/if}
