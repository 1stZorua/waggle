<script lang="ts">
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';
	import { Text } from '../index';

	let {
		tag = 'span',
		className,
		children
	}: { tag?: keyof HTMLElementTagNameMap; className?: ClassType; children?: Snippet } = $props();
</script>

<Text {tag} {className} props={{ size: 'xl', weight: 'semibold' }}>
	{@render children?.()}
</Text>
