<script lang="ts">
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';
	import { Card } from '../index';

	interface Props {
		className?: ClassType;
		children?: Snippet;
	}

	let { className, children, ...rest }: Props = $props();
</script>

<Card {className} props={{ variant: 'secondary' }} {...rest}>
	{@render children?.()}
</Card>
