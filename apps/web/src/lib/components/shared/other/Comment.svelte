<script lang="ts">
	import { timeAgo } from '$lib/utils/format';
	import { TextBase, TextSmall } from '../text';
	import { Avatar, Icon, Ellipse, Separator } from '.';
	import { ButtonText } from '../buttons';
	import type { Comment } from '$lib/types/types';

	interface Props {
		comment: Comment;
		enableReplies?: boolean;
	}

	let showReplies = $state(false);

	let { comment, enableReplies = true }: Props = $props();

	function toggleReplies() {
		showReplies = !showReplies;
	}
</script>

<div class="gap-sm flex min-w-0 flex-col">
	<div class="gap-sm flex min-w-0 items-start justify-between">
		<div class="gap-xs flex min-w-0 flex-1 items-start">
			<Avatar className="h-10 w-10" src={'/images/avatar.png'} alt="avatar" />
			<div class="gap-sm flex min-w-0 flex-1 flex-col">
				<div>
					<div class="gap-sm flex items-center">
						<TextBase className="truncate">
							{comment.user?.name}
						</TextBase>
						<Ellipse />
						<TextSmall className="text-secondary whitespace-nowrap">
							{comment.createdAt ? timeAgo(comment.createdAt) : 'just now'}
						</TextSmall>
						<ButtonText className="group self-center ml-auto">
							<Icon
								className="stroke-tertiary text-sm group-hover:stroke-pink-light group-hover:text-pink stroke-2 text-transparent"
								icon="iconoir:heart-solid"
							/>
						</ButtonText>
					</div>
					<TextSmall className="font-normal">{comment.content}</TextSmall>
				</div>
				{#if showReplies && (comment.commentIds?.length ?? 0 > 0)}
					<div class="gap-sm flex flex-col">
						{#each comment.commentIds as id}
							<div class="text-secondary text-sm italic">
								Reply ID: {id} (not loaded)
							</div>
						{/each}
					</div>
				{/if}

				{#if (comment.commentIds?.length ?? 0 > 0) && enableReplies}
					<ButtonText className="group text-secondary gap-xs" onclick={toggleReplies}>
						<Separator className="w-5"></Separator>
						<TextSmall className="font-normal">{showReplies ? 'Hide' : 'Show'} replies</TextSmall>
					</ButtonText>
				{/if}
			</div>
		</div>
	</div>
</div>
