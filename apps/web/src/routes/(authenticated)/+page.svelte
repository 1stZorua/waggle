<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { CardPrimary } from '$components/shared/cards';
	import { Avatar, Ellipse, Icon, Separator } from '$components/shared/other';
	import { TextBase, TextSmall } from '$components/shared/text';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();

	function timeAgo(dateString: string) {
		const now = new Date();
		const date = new Date(dateString);
		const diffMs = now.getTime() - date.getTime();

		const diffSec = Math.floor(diffMs / 1000);
		if (diffSec < 60) return `${diffSec} s`;

		const diffMin = Math.floor(diffSec / 60);
		if (diffMin < 60) return `${diffMin} m`;

		const diffHours = Math.floor(diffMin / 60);
		if (diffHours < 24) return `${diffHours} h`;

		const diffDays = Math.floor(diffHours / 24);
		return `${diffDays}d`;
	}
</script>

<section class="gap-md flex h-full">
	<div class="flex-2/3">
		<div></div>
		<div class="shadow-ui flex flex-col rounded-lg">
			{#await data.posts}
				<p>Loading posts...</p>
			{:then posts}
				<div>Loaded</div>
				<!-- {#each posts.toReversed() as post, index}
					<CardPrimary className="flex-col shadow-none!">
						<div class="flex w-full items-center justify-between">
							<div class="gap-sm flex items-center">
								<Avatar className="h-10 w-10" src="images/avatar.png" alt="Avatar" />
								<div class="flex flex-col">
									<TextBase>{post.user?.firstName} {post.user?.lastName}</TextBase>
									<TextSmall className="text-secondary">@{post.user?.username}</TextSmall>
								</div>
								<Ellipse></Ellipse>
								<TextSmall className="text-secondary">{timeAgo(post.createdAt as string)}</TextSmall
								>
							</div>
							<ButtonText>
								<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
							</ButtonText>
						</div>
						<img
							class="aspect-square rounded-lg object-cover"
							src={post.mediaIds ? post.mediaUrls?.[post.mediaIds[0]]?.url : ''}
							alt="animal"
						/>
						<div class="flex justify-between">
							<div class="gap-md flex">
								<ButtonText className="group gap-xs">
									<Icon
										className="stroke-2 stroke-tertiary text-transparent text-lg group-hover:stroke-pink-light group-hover:text-pink"
										icon="iconoir:heart-solid"
									></Icon>
									<TextSmall className="text-secondary">1.1K</TextSmall>
								</ButtonText>
								<ButtonText className="group gap-xs">
									<Icon
										className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
										icon="iconamoon:comment-fill"
									></Icon>
									<TextSmall className="text-secondary">107</TextSmall>
								</ButtonText>
							</div>
							<ButtonText className="group">
								<Icon
									className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-blue-light group-hover:text-blue"
									icon="material-symbols:bookmark-rounded"
								></Icon>
							</ButtonText>
						</div>
						{#if index < posts.length - 1}
							<Separator className="mt-5"></Separator>
						{/if}
					</CardPrimary>
				{/each} -->
			{:catch error}
				<p>Error loading posts: {error.message}</p>
			{/await}
		</div>
	</div>
	<div class="gap-md action-bar fixed right-0 flex flex-1/3 flex-col">
		<CardPrimary className="h-full items-start">
			<div class="flex w-full items-center justify-between">
				<TextBase>Top Companions</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>
		</CardPrimary>
		<CardPrimary className="items-start h-full">
			<div class="flex w-full items-center justify-between">
				<TextBase>Buddies</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>
		</CardPrimary>
	</div>
</section>
