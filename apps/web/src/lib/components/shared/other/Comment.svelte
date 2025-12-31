<script lang="ts">
	import { onMount } from 'svelte';
	import { formatNumber, formatText, formatTimestamp, timeAgo } from '$lib/utils/format';
	import { useToggleReaction } from '$lib/hooks/useToggleReaction.svelte';
	import { useCommentModal } from '$lib/hooks/useCommentModal.svelte';
	import { TextBase, TextSmall } from '../text';
	import { Avatar, Icon, Ellipse, Separator, InfiniteScroll } from '.';
	import { ButtonText } from '../buttons';
	import type { Comment as CommentType } from '$lib/types/types';
	import { cn } from '$lib/utils/merge';
	import Comment from './Comment.svelte';
	import { page } from '$app/state';

	interface Props {
		comment: CommentType;
		enableReplies?: boolean;
		onRefresh?: () => void;
	}

	let user = page.data.user;

	let { comment, enableReplies = true, onRefresh }: Props = $props();

	let showReplies = $state(false);
	let like = useToggleReaction(comment?.id, 'likes', 'comment', comment?.likeCount ?? 0);
	let replies = $state<CommentType[]>([]);
	let nextCursor = $state<string | null>(null);
	let loadingReplies = $state(false);

	const isOwner = $derived(user?.id === comment.userId);

	onMount(() => {
		if (!comment?.id) return;
		like.init();
	});

	async function toggleReplies() {
		showReplies = !showReplies;

		if (showReplies && replies.length === 0 && nextCursor === null) {
			await loadReplies();
		}
	}

	async function loadReplies() {
		if (!comment?.id || loadingReplies) return;

		loadingReplies = true;

		try {
			const url = nextCursor
				? `/api/comments/${comment.id}/replies?cursor=${nextCursor}`
				: `/api/comments/${comment.id}/replies`;

			const res = await fetch(url);
			const result = await res.json();

			replies = [...replies, ...result.items];
			nextCursor = result.nextCursor;
		} catch (e) {
			console.error('Failed to load replies', e);
		} finally {
			loadingReplies = false;
		}
	}

	function openReplyModal() {
		useCommentModal.openCreate(comment, onRefresh);
	}

	function openActionsModal() {
		useCommentModal.openActions(comment, onRefresh);
	}
</script>

<div class="gap-sm flex min-w-0 flex-col">
	<div class="gap-sm flex min-w-0 items-start justify-between">
		<div class="gap-xs flex min-w-0 flex-1 items-start">
			<ButtonText href="/users/{comment.user?.id}">
				<Avatar
					className="h-10 w-10"
					src={comment.user?.avatarUrl?.url ?? '/images/anonymous.png'}
					alt="avatar"
				/>
			</ButtonText>
			<div class="gap-sm flex min-w-0 flex-1 flex-col">
				<div>
					<div class="gap-sm flex items-center">
						<TextBase className="truncate">
							{comment.user?.firstName}
							{comment.user?.lastName}
						</TextBase>
						<Ellipse />
						<TextSmall className="text-secondary whitespace-nowrap">
							{comment.createdAt ? timeAgo(comment.createdAt) : 'just now'}
						</TextSmall>
						{#if enableReplies}
							{#if isOwner}
								<ButtonText onclick={openActionsModal} className="group self-center ml-auto pr-0.5">
									<Icon className="text-secondary text-sm" icon="uis:ellipsis-h" />
								</ButtonText>
							{:else}
								<ButtonText onclick={like.toggle} className="group self-center ml-auto pr-0.5">
									<Icon
										className={cn(
											'stroke-2 text-sm',
											like.active
												? 'stroke-pink text-pink'
												: 'stroke-tertiary text-transparent group-hover:stroke-pink-light group-hover:text-pink'
										)}
										icon="iconoir:heart-solid"
									/>
								</ButtonText>
							{/if}
						{/if}
					</div>
					<TextSmall className="font-normal">{comment.content}</TextSmall>
				</div>

				{#if showReplies}
					<InfiniteScroll
						items={replies}
						hasMore={!!nextCursor}
						isLoading={loadingReplies}
						onLoadMore={loadReplies}
					>
						{#snippet children({ items })}
							<div class="gap-sm flex flex-col">
								{#each items as reply}
									<Comment comment={reply} {onRefresh} />
								{/each}
							</div>
						{/snippet}

						{#snippet loadingContent()}
							<div class="py-2 text-center">
								<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
							</div>
						{/snippet}
					</InfiniteScroll>
				{/if}

				{#if enableReplies}
					<div class="gap-xs text-secondary flex">
						<TextSmall>{formatTimestamp(comment?.createdAt as string)}</TextSmall>
						{#if like.count}
							<TextSmall>{formatText(like.count, formatNumber(like.count), 'like')}</TextSmall>
						{/if}
						<ButtonText onclick={openReplyModal}>
							<TextSmall>Reply</TextSmall>
						</ButtonText>
					</div>
					{#if comment.replyCount}
						<ButtonText className="group text-secondary gap-xs" onclick={toggleReplies}>
							<Separator className="w-5"></Separator>
							<TextSmall className="font-normal">
								{showReplies ? 'Hide' : 'Show'} replies ({formatNumber(comment.replyCount)})
							</TextSmall>
						</ButtonText>
					{/if}
				{/if}
			</div>
		</div>
	</div>
</div>
