<script lang="ts">
	import { onMount } from 'svelte';
	import { CardPrimary } from '../../cards';
	import { TextBase, TextSmall } from '../../text';
	import { Avatar, Icon, Ellipse, Separator } from '../../other';
	import { timeAgo, formatNumber } from '$lib/utils/format';
	import { ButtonText } from '../../buttons';
	import type { Post as PostType } from '$lib/types/types';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';

	interface Props {
		className?: ClassType;
		post: PostType | null;
		isLast: boolean;
	}

	let { className, post, isLast }: Props = $props();

	// Generic toggle state for likes/favorites
	type ToggleState<T extends { id: string }> = {
		active: boolean;
		obj: T | null; // stores Like or Favorite object
		count?: number; // optional, only for things like likes
	};

	const createToggle = <T extends { id: string }>(count?: number): ToggleState<T> => ({
		active: false,
		obj: null,
		count
	});

	let like = $state<ToggleState<{ id: string }>>(createToggle(post?.likeCount ?? 0));
	let favorite = $state<ToggleState<{ id: string }>>(createToggle());

	// Initialize on mount
	onMount(() => {
		if (!post?.id) return;
		initToggle('likes', like);
		initToggle('favorites', favorite);
	});

	// Fetch backend object for toggle (like/favorite)
	async function initToggle<T extends { id: string }>(
		type: 'likes' | 'favorites',
		state: ToggleState<T>
	) {
		try {
			const res = await fetch(`/api/${type}/check/${post?.id}`);
			const data = await res.json(); // returns object or null

			state.active = !!data;
			state.obj = data ?? null;
		} catch (e) {
			console.error(`Failed to check ${type}`, e);
		}
	}

	// Toggle like/favorite
	async function toggle<T extends { id: string }>(
		type: 'likes' | 'favorites',
		state: ToggleState<T>
	) {
		if (!post?.id) return;

		const prev = { ...state };
		state.active = !state.active;
		if (state.count !== undefined) state.count += state.active ? 1 : -1;

		try {
			if (state.active) {
				const res = await fetch(`/api/${type}`, {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ targetId: post.id, targetType: 'Post' })
				});
				state.obj = (await res.json())?.data ?? null;
			} else {
				await fetch(`/api/${type}`, {
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ id: state.obj?.id }) // safe because T extends {id:string}
				});
				state.obj = null;
			}
		} catch (e) {
			console.error(`Failed to toggle ${type}`, e);
			Object.assign(state, prev);
		}
	}
</script>

{#if post}
	<CardPrimary className={cn('flex-col', className)}>
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
			<ButtonText href={`/posts/${post.id}`}>
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
				<ButtonText className="group gap-xs" onclick={() => toggle('likes', like)}>
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
				<ButtonText className="group gap-xs">
					<Icon
						className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
						icon="iconamoon:comment-fill"
					/>
					<TextSmall className="text-secondary">{formatNumber(post?.commentCount)}</TextSmall>
				</ButtonText>
			</div>
			<ButtonText className="group" onclick={() => toggle('favorites', favorite)}>
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

		{#if isLast}
			<Separator></Separator>
		{/if}
	</CardPrimary>
{/if}
