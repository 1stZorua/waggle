<script lang="ts">
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';
	import { Card } from '../index';

	interface Props {
		className?: ClassType;
		href?: string;
		children?: Snippet;
	}

	let { className, href, children, ...rest }: Props = $props();
</script>

<Card {className} props={{ variant: 'primary' }} {...rest}>
	{@render children?.()}
</Card>
