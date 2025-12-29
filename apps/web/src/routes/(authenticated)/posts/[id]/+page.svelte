<script lang="ts">
	import { Post } from '$components/shared/posts';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<section class="h-full w-full">
	<div class="h-full w-full max-w-240 min-w-0 justify-self-center max-xl:max-w-max">
		{#await data.post}
			<div>loading</div>
		{:then post}
			<Post {post} variant="detail"></Post>
		{/await}
	</div>
</section>
