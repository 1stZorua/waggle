<script lang="ts">
	import type { Snippet } from 'svelte';

	interface Props {
		show: boolean;
		direction?: 'bottom-right' | 'bottom-left' | 'top-right' | 'top-left' | 'left' | 'right';
		width?: string;
		className?: string;
		children: Snippet;
	}

	let {
		show = $bindable(),
		direction = 'bottom-right',
		width = 'w-32',
		className = '',
		children
	}: Props = $props();

	const positionClasses = {
		'bottom-right': 'top-full right-0 mt-2',
		'bottom-left': 'top-full left-0 mt-2',
		'top-right': 'bottom-full right-0 mb-2',
		'top-left': 'bottom-full left-0 mb-2',
		left: 'right-full top-0 mr-2',
		right: 'left-full top-0 ml-2'
	};
</script>

{#if show}
	<div
		class="bg-background shadow-ui absolute z-50 overflow-hidden rounded-lg {width} {positionClasses[
			direction
		]} {className}"
	>
		{@render children()}
	</div>
{/if}
