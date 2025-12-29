<script lang="ts">
	import type { ClassType } from '$components/_types';
	import { cn } from '$lib/utils/merge';
	import { Button } from '../buttons';
	import { TextBase } from '../text';

	let {
		className,
		items,
		changeTab
	}: { className?: ClassType; items: string[]; changeTab: (index: number) => void } = $props();

	let activeIndex = $state(0);
	let tabTransition = $state(false);

	function onTabChange(index: number) {
		if (activeIndex === index) return;
		if (document.startViewTransition) {
			tabTransition = true;

			document
				.startViewTransition(() => {
					activeIndex = index;
					changeTab(index);
				})
				.finished.finally(() => {
					tabTransition = false;
				});
			return;
		}

		activeIndex = index;
		changeTab(index);
	}
</script>

<div class={cn(className, 'gap-sm flex')}>
	{#each items as item, index}
		{@const isActive = activeIndex === index}
		<Button
			props={{ size: 'sm', variant: 'text' }}
			className={cn(
				'relative px-6 hover:border-primary hover:text-primary',
				isActive ? 'text-primary' : 'text-secondary'
			)}
			onclick={() => onTabChange(index)}
		>
			<TextBase>{item}</TextBase>
			{#if isActive}
				<span
					class={cn(
						'bg-primary absolute bottom-0 left-0 h-0.5 w-full',
						tabTransition ? '[view-transition-name:tab]' : ''
					)}
				></span>
			{/if}
		</Button>
	{/each}
</div>
