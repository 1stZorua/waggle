<script lang="ts">
	import type { ClassType } from '$components/_types';
	import type { Snippet } from 'svelte';
	import { Tag } from '../index';

	interface Props {
		className?: ClassType;
		children?: Snippet;
	}

	let { className, children, ...rest }: Props = $props();
</script>

<Tag {className} props={{ variant: 'error' }} {...rest}>
	{@render children?.()}
</Tag>
