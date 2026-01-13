<script lang="ts">
	import { onMount } from 'svelte';
	import { useToggleReaction } from '$lib/hooks/useToggleReaction.svelte';
	import { useCommentModal } from '$lib/hooks/useCommentModal.svelte';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';
	import { CardPrimary } from '../../cards';
	import { TextBase, TextSmall } from '../../text';
	import { Avatar, Icon, Ellipse, Separator, Comment, InfiniteScroll, Carousel } from '../../other';
	import { formatNumber, timeAgo } from '$lib/utils/format';
	import { ButtonText } from '../../buttons';
	import type { Post as PostType } from '$lib/types/types';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	interface Props {
		className?: ClassType;
		post: PostType | null;
	}

	let { className, post = $bindable() }: Props = $props();

	let like = useToggleReaction(post?.id, 'likes', 'post', post?.likeCount ?? 0);
	let favorite = useToggleReaction(post?.id, 'favorites', 'post');

	let comments = $state(post?.comments || []);
	let nextCursor = $state<string | null>(post?.commentsNextCursor ?? null);
	let loadingMore = $state(false);

	const postAsComment = $derived({
		content: post?.caption,
		userId: post?.userId,
		postId: post?.id,
		createdAt: post?.createdAt,
		user: post?.user
	});

	const postImages = $derived(
		post?.mediaUrls ? Object.values(post.mediaUrls).map((media) => media.url) : []
	);

	onMount(() => {
		if (!post?.id) return;
		like.init();
		favorite.init();
	});

	async function loadMoreComments() {
		if (!post?.id || !nextCursor || loadingMore) return;

		loadingMore = true;

		try {
			const res = await fetch(`/api/comments/posts/${post.id}?cursor=${nextCursor}`);
			const result = await res.json();

			comments = [...comments, ...result.items];
			nextCursor = result.nextCursor;
		} finally {
			loadingMore = false;
		}
	}

	async function refreshComments() {
		if (!post?.id) return;

		try {
			const res = await fetch(`/api/comments/posts/${post.id}`);
			const result = await res.json();

			comments = result.items;
			nextCursor = result.nextCursor;
		} catch (error) {
			console.error('Failed to refresh comments:', error);
		}
	}

	async function refreshPost() {
		if (!post?.id) return;

		try {
			const res = await fetch(`/api/posts/${post.id}`);
			const result = await res.json();

			if (result) {
				post = result;
			}
		} catch (error) {
			console.error('Failed to refresh post:', error);
		}
	}

	function openCommentModal() {
		useCommentModal.openCreate(
			{
				postId: post?.id
			},
			refreshComments
		);
	}

	function openPostModal() {
		if (post) usePostModal.openActions(post, refreshPost);
	}
</script>

{#if post}
	{@const user = post?.user}
	<CardPrimary className={cn('hidden xl:flex max-h-[640px] h-full items-start min-w-0', className)}>
		<div class="h-full w-auto min-w-0 flex-1">
			{#if postImages.length > 0}
				<Carousel
					images={postImages as string[]}
					imageClassName="h-full w-full max-w-full rounded-lg object-cover object-[50%_35%]"
				/>
			{/if}
		</div>
		<div class="gap-sm flex h-full max-w-[500px] min-w-0 flex-1 flex-col justify-between">
			<div class="gap-sm flex min-h-0 min-w-0 flex-1 flex-col">
				<div class="flex w-full min-w-0 items-center justify-between">
					<a href="/users/{user?.id}" class="gap-sm flex min-w-0 items-center">
						<Avatar src={user?.avatarUrl?.url ?? '/images/anonymous.png'} alt="Avatar" />
						<div class="flex min-w-0 flex-col">
							<TextBase className="truncate">{user?.firstName} {user?.lastName}</TextBase>
							<TextSmall className="text-secondary truncate font-secondary font-normal"
								>@{user?.username}</TextSmall
							>
						</div>
					</a>
					<ButtonText onclick={openPostModal}>
						<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
					</ButtonText>
				</div>
				<Separator></Separator>
				<div
					class="gap-md scrollbar-hidden flex min-h-0 min-w-0 flex-1 flex-col overflow-y-auto"
					data-infinite-scroll
				>
					<Comment comment={postAsComment} enableReplies={false}></Comment>

					<InfiniteScroll
						items={comments}
						hasMore={!!nextCursor}
						isLoading={loadingMore}
						onLoadMore={loadMoreComments}
					>
						{#snippet children({ items })}
							{#each items as comment (comment.id)}
								<Comment {comment} onRefresh={refreshComments} />
							{/each}
						{/snippet}

						{#snippet loadingContent()}
							<div class="py-4 text-center">
								<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
							</div>
						{/snippet}
					</InfiniteScroll>
				</div>
			</div>
			<div class="flex min-w-0 justify-between">
				<div class="gap-md flex">
					<ButtonText className="group gap-xs" onclick={like.toggle}>
						<Icon
							className={cn(
								'stroke-2 text-lg',
								like.active
									? 'stroke-pink-light text-pink'
									: 'stroke-tertiary text-transparent group-hover:stroke-pink-light group-hover:text-pink'
							)}
							icon="iconoir:heart-solid"
						/>
						<TextSmall className="text-secondary">{formatNumber(like.count)}</TextSmall>
					</ButtonText>
					<ButtonText className="group gap-xs" onclick={openCommentModal}>
						<Icon
							className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
							icon="iconamoon:comment-fill"
						/>
						<TextSmall className="text-secondary">{formatNumber(post?.commentCount)}</TextSmall>
					</ButtonText>
				</div>
				<ButtonText className="group" onclick={favorite.toggle}>
					<Icon
						className={cn(
							'text-lg stroke-2',
							favorite.active
								? 'text-blue stroke-blue-light'
								: 'text-transparent stroke-tertiary group-hover:text-blue'
						)}
						icon="material-symbols:bookmark-rounded"
					/>
				</ButtonText>
			</div>
		</div>
	</CardPrimary>

	<CardPrimary className={cn('flex-col xl:hidden', className)}>
		<div class="flex w-full items-center justify-between">
			<div class="gap-sm flex items-center">
				<Avatar src={user?.avatarUrl?.url ?? '/images/anonymous.png'} alt="Avatar" />
				<div class="flex flex-col">
					<TextBase>{user?.firstName} {user?.lastName}</TextBase>
					<TextSmall className="text-secondary">@{user?.username}</TextSmall>
				</div>
				<Ellipse></Ellipse>
				<TextSmall className="text-secondary">{timeAgo(post.createdAt as string)}</TextSmall>
			</div>
			<ButtonText onclick={openPostModal}>
				<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
			</ButtonText>
		</div>

		{#if postImages.length > 0}
			<Carousel
				images={postImages as string[]}
				imageClassName="aspect-4/5 max-h-[640px] w-full rounded-lg object-cover object-[50%_35%]"
			/>
		{/if}

		<div class="flex justify-between">
			<div class="gap-md flex">
				<ButtonText className="group gap-xs" onclick={like.toggle}>
					<Icon
						className={cn(
							'stroke-2 text-lg',
							like.active
								? 'stroke-pink-light text-pink'
								: 'stroke-tertiary text-transparent group-hover:stroke-pink-light group-hover:text-pink'
						)}
						icon="iconoir:heart-solid"
					/>
					<TextSmall className="text-secondary">{formatNumber(like.count)}</TextSmall>
				</ButtonText>
				<ButtonText className="group gap-xs" onclick={openCommentModal}>
					<Icon
						className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
						icon="iconamoon:comment-fill"
					/>
					<TextSmall className="text-secondary">{formatNumber(post?.commentCount)}</TextSmall>
				</ButtonText>
			</div>
			<ButtonText className="group" onclick={favorite.toggle}>
				<Icon
					className={cn(
						'text-lg stroke-2',
						favorite.active
							? 'text-blue stroke-blue-light'
							: 'text-transparent stroke-tertiary group-hover:text-blue'
					)}
					icon="material-symbols:bookmark-rounded"
				/>
			</ButtonText>
		</div>

		<TextBase className="font-normal">{post?.caption}</TextBase>

		<div class="gap-sm flex h-[300px]">
			<div class="gap-sm flex min-h-0 min-w-0 flex-1 flex-col">
				<Separator></Separator>
				<div
					class="gap-md scrollbar-hidden flex min-h-0 min-w-0 flex-1 flex-col overflow-y-auto"
					data-infinite-scroll
				>
					<InfiniteScroll
						items={comments}
						hasMore={!!nextCursor}
						isLoading={loadingMore}
						onLoadMore={loadMoreComments}
					>
						{#snippet children({ items })}
							{#each items as comment (comment.id)}
								<Comment {comment} onRefresh={refreshComments} />
							{/each}
						{/snippet}

						{#snippet loadingContent()}
							<div class="py-4 text-center">
								<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
							</div>
						{/snippet}
					</InfiniteScroll>
				</div>
			</div>
		</div>
	</CardPrimary>
{/if}
