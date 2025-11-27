<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';

	let {
		tag = 'div',
		className,
		orientation = 'horizontal',
		children
	}: {
		tag?: keyof HTMLElementTagNameMap;
		className?: ClassType;
		orientation?: 'vertical' | 'horizontal';
		children?: Snippet;
	} = $props();
</script>

<svelte:element
	this={tag}
	class={cn(
		'gap-xs border-accent flex items-center',
		orientation === 'horizontal' ? 'flex-row' : 'flex-col',
		className
	)}
>
	<div
		class={cn(
			'flex-1 rounded-full border-inherit',
			orientation === 'horizontal' ? 'border-t-2' : 'border-l-2'
		)}
	></div>
	{#if children}
		<span>{@render children?.()}</span>
		<div
			class={cn(
				'flex-1 rounded-full border-inherit',
				orientation === 'horizontal' ? 'border-t-2' : 'border-l-2'
			)}
		></div>
	{/if}
</svelte:element>
