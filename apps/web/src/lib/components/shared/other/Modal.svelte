<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';

	interface Props {
		className?: ClassType;
		children?: Snippet;
		isOpen: boolean;
	}

	let backdrop: HTMLDivElement | undefined = $state();
	let { className, children, isOpen = $bindable() }: Props = $props();

	function onClose() {
		isOpen = false;
	}
</script>

{#if isOpen}
	<div
		role="presentation"
		bind:this={backdrop}
		class="fixed inset-0 z-50 flex h-full w-full max-w-full items-center justify-center bg-black/10"
		onclick={(e: MouseEvent) => e.target === backdrop && onClose()}
		onkeydown={(e: KeyboardEvent) => {}}
	>
		<div class={cn('bg-background wrapper shadow-ui min-h-max min-w-max rounded-xl', className)}>
			<!-- <div class="gap-xl flex items-center justify-between">
				<TextBase>{title}</TextBase>
				{#if showClose}
					<ButtonText onclick={onClose}>
						<Icon icon="material-symbols:close-rounded"></Icon>
					</ButtonText>
				{/if}
				{@render header?.()}
			</div> -->
			{@render children?.()}
		</div>
	</div>
{/if}
