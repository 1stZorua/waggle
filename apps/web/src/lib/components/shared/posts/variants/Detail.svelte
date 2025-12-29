<script lang="ts">
	import { CardPrimary } from '../../cards';
	import { TextBase, TextSmall } from '../../text';
	import { Avatar, Icon, Ellipse, Separator, Comment } from '../../other';
	import { formatNumber, timeAgo } from '$lib/utils/format';
	import { ButtonText } from '../../buttons';
	import type { Post as PostType } from '$lib/types/types';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	interface Props {
		className?: ClassType;
		post: PostType | null;
	}

	let { className, post }: Props = $props();
</script>

{#if post}
	{@const commentCount = formatNumber(post?.likeCount)}
	{@const likeCount = formatNumber(post?.commentCount)}
	<CardPrimary className={cn('hidden xl:flex max-h-[640px] h-full items-start min-w-0', className)}>
		<div class="aspect-4/5 h-full w-full max-w-[620px] min-w-0 flex-1">
			<img
				class="h-full w-full rounded-lg object-cover object-[50%_35%]"
				src={post.mediaUrls?.[post.thumbnailId]?.url}
				alt="post"
			/>
		</div>
		<div class="gap-sm flex h-full min-w-0 flex-1 flex-col justify-between">
			<div class="gap-sm flex min-h-0 min-w-0 flex-1 flex-col">
				<div class="flex w-full min-w-0 items-center justify-between">
					<div class="gap-sm flex min-w-0 items-center">
						<Avatar src="/images/avatar.png" alt="Avatar" />
						<div class="flex min-w-0 flex-col">
							<TextBase className="truncate">{post.user?.firstName} {post.user?.lastName}</TextBase>
							<TextSmall className="text-secondary truncate">@{post.user?.username}</TextSmall>
						</div>
						<Ellipse></Ellipse>
						<TextSmall className="text-secondary whitespace-nowrap"
							>{timeAgo(post.createdAt as string)}</TextSmall
						>
					</div>
					<ButtonText>
						<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
					</ButtonText>
				</div>
				<TextSmall className="font-normal">{post?.caption}</TextSmall>
				<Separator></Separator>
				<div class="gap-xs flex min-h-0 min-w-0 flex-1 flex-col overflow-y-auto pr-4">
					{#each Array(20) as _}
						<Comment
							comment={{
								id: _,
								content: 'comment',
								user: {
									id: post.user?.id!,
									email: post.user?.email!,
									name: `${post.user?.firstName!} ${post.user?.lastName}`,
									username: post?.user?.username!,
									roles: []
								},
								commentIds: [1, 2, 3],
								createdAt: post?.createdAt
							}}
						/>
					{/each}
				</div>
			</div>
			<div class="flex min-w-0 justify-between">
				<div class="gap-md flex">
					<ButtonText className="group gap-xs">
						<Icon
							className="stroke-2 stroke-tertiary text-transparent text-lg group-hover:stroke-pink-light group-hover:text-pink"
							icon="iconoir:heart-solid"
						/>
						<TextSmall className="text-secondary">{likeCount}</TextSmall>
					</ButtonText>
					<ButtonText className="group gap-xs">
						<Icon
							className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
							icon="iconamoon:comment-fill"
						/>
						<TextSmall className="text-secondary">{commentCount}</TextSmall>
					</ButtonText>
				</div>
				<ButtonText className="group">
					<Icon
						className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-blue-light group-hover:text-blue"
						icon="material-symbols:bookmark-rounded"
					/>
				</ButtonText>
			</div>
		</div>
	</CardPrimary>

	<CardPrimary className={cn('flex-col xl:hidden', className)}>
		<div class="flex w-full items-center justify-between">
			<div class="gap-sm flex items-center">
				<Avatar src="/images/avatar.png" alt="Avatar" />
				<div class="flex flex-col">
					<TextBase>{post.user?.firstName} {post.user?.lastName}</TextBase>
					<TextSmall className="text-secondary">@{post.user?.username}</TextSmall>
				</div>
				<Ellipse></Ellipse>
				<TextSmall className="text-secondary">{timeAgo(post.createdAt as string)}</TextSmall>
			</div>
			<ButtonText>
				<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
			</ButtonText>
		</div>

		<img
			class="aspect-4/5 max-h-[640px] w-full rounded-lg object-cover object-[50%_35%]"
			src={post.mediaUrls?.[post.thumbnailId]?.url}
			alt="post"
		/>

		<div class="flex justify-between">
			<div class="gap-md flex">
				<ButtonText className="group gap-xs">
					<Icon
						className="stroke-2 stroke-tertiary text-transparent text-lg group-hover:stroke-pink-light group-hover:text-pink"
						icon="iconoir:heart-solid"
					/>
					<TextSmall className="text-secondary">{likeCount}</TextSmall>
				</ButtonText>
				<ButtonText className="group gap-xs">
					<Icon
						className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
						icon="iconamoon:comment-fill"
					/>
					<TextSmall className="text-secondary">{commentCount}</TextSmall>
				</ButtonText>
			</div>
			<ButtonText className="group">
				<Icon
					className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-blue-light group-hover:text-blue"
					icon="material-symbols:bookmark-rounded"
				/>
			</ButtonText>
		</div>

		<TextBase className="font-normal">{post?.caption}</TextBase>
	</CardPrimary>
{/if}
