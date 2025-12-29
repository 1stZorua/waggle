<script lang="ts">
	import type { ClassType } from '$components/_types';
	import type { Post as PostType } from '$lib/types/types';
	import { PostDetail, PostFeed } from '.';

	interface Props {
		className?: ClassType;
		post: PostType | null;
		variant?: 'feed' | 'detail';
		isLast?: boolean;
	}

	let { className, post, variant = 'feed', isLast = false }: Props = $props();
</script>

{#if variant === 'feed'}
	<PostFeed {className} {post} {isLast} />
{:else if variant === 'detail'}
	<PostDetail {className} {post} />
{/if}
