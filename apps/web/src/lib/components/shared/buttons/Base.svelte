<script lang="ts">
	import type { Snippet } from 'svelte';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import { type ButtonProps, buttonVariants } from './variants';
	import type { HTMLAnchorAttributes, HTMLButtonAttributes } from 'svelte/elements';

	interface Props extends HTMLButtonAttributes {
		className?: ClassType;
		href?: string;
		props?: ButtonProps;
		children?: Snippet;
	}

	let { className, href, props, children, ...rest }: Props = $props();
</script>

{#if href}
	<a {href} class={cn(buttonVariants({ ...props }), className)} {...rest as HTMLAnchorAttributes}>
		{@render children?.()}
	</a>
{:else}
	<button class={cn(buttonVariants({ ...props }), className)} {...rest as HTMLButtonAttributes}>
		{@render children?.()}
	</button>
{/if}
