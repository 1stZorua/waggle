<script lang="ts">
	import type { Snippet } from 'svelte';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import { type CardProps, cardVariants } from './variants';
	import type { HTMLAnchorAttributes, HTMLButtonAttributes } from 'svelte/elements';

	interface Props extends Partial<HTMLAnchorAttributes & HTMLButtonAttributes> {
		tag?: keyof HTMLElementTagNameMap;
		className?: ClassType;
		props?: CardProps;
		children?: Snippet;
	}

	let { tag = 'div', className, href, props, children, ...rest }: Props = $props();
</script>

<svelte:element this={tag} {href} class={cn(cardVariants(props), className)} {...rest}>
	{@render children?.()}
</svelte:element>
