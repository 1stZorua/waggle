<script lang="ts">
	import type { Snippet } from 'svelte';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import { type TextProps, textVariants } from './variants';

	let {
		tag = 'span',
		className,
		props,
		children
	}: {
		tag?: keyof HTMLElementTagNameMap;
		className?: ClassType;
		props?: TextProps;
		children?: Snippet;
	} = $props();
</script>

<svelte:element this={tag} class={cn(textVariants(props), className)}>
	{@render children?.()}
</svelte:element>
