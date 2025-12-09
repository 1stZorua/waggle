<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';
	import { tagVariants, type TagProps } from './variants';

	interface Props {
		className?: ClassType;
		props?: TagProps;
		children?: Snippet;
	}

	let { className, props, children }: Props = $props();
</script>

<div class={cn(tagVariants(props), className)}>
	{@render children?.()}
</div>
