<script lang="ts">
	import { onMount } from 'svelte';
	import { preloadData, pushState, goto } from '$app/navigation';
	import { useToggleReaction } from '$lib/hooks/useToggleReaction.svelte';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';
	import { CardPrimary } from '../../cards';
	import { TextBase, TextSmall } from '../../text';
	import { Avatar, Icon, Ellipse, Separator, Carousel } from '../../other';
	import { timeAgo, formatNumber } from '$lib/utils/format';
	import { ButtonText } from '../../buttons';
	import type { Post as PostType } from '$lib/types/types';
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import { page } from '$app/state';

	interface Props {
		className?: ClassType;
		post: PostType | null;
		isLast: boolean;
	}

	let { className, post, isLast }: Props = $props();

	let like = useToggleReaction(post?.id, 'likes', 'post', post?.likeCount ?? 0);
	let favorite = useToggleReaction(post?.id, 'favorites', 'post');

	const postImages = $derived(
		post?.mediaUrls ? Object.values(post.mediaUrls).map((media) => media.url) : []
	);

	onMount(() => {
		if (!post?.id) return;
		like.init();
		favorite.init();
	});

	async function openViewPostModal(e: MouseEvent) {
		const isSmallScreen = window.matchMedia('(max-width:1280px)').matches;
		if (isSmallScreen) return;
		e.preventDefault();

		const { href } = e.currentTarget as HTMLAnchorElement;
		const result = await preloadData(href);

		if (result.type === 'loaded' && result.status === 200) {
			pushState(href, { post: (await result.data['post']) as PostType });
		} else {
			goto(href);
		}
	}

	function openPostModal(e: MouseEvent) {
		e.preventDefault();
		e.stopPropagation();
		if (post) {
			usePostModal.openActions(post);
		}
	}
</script>

{#if post}
	<CardPrimary className={cn('flex-col', className)}>
		<div class="gap-sm flex w-full items-center justify-between">
			<a href="/users/{post.user?.id}" class="gap-sm flex items-center">
				<Avatar src={post.user?.avatarUrl?.url ?? '/images/anonymous.png'} alt="Avatar" />
				<div class="flex flex-col">
					<TextBase>{post.user?.firstName} {post.user?.lastName}</TextBase>
					<TextSmall className="text-secondary font-secondary font-normal"
						>@{post.user?.username}</TextSmall
					>
				</div>
				<Ellipse></Ellipse>
				<TextSmall className="text-secondary">{timeAgo(post.createdAt as string)}</TextSmall>
			</a>
			<ButtonText onclick={openPostModal}>
				<Icon className="text-secondary text-lg" icon="uis:ellipsis-h"></Icon>
			</ButtonText>
		</div>

		<a href={`/posts/${post.id}`} onclick={openViewPostModal}>
			{#if postImages.length > 0}
				<Carousel
					images={postImages as string[]}
					imageClassName="aspect-4/5 max-h-[640px] w-full rounded-lg object-cover object-[50%_35%]"
				/>
			{/if}
		</a>

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
				<a href={`/posts/${post.id}`} onclick={openViewPostModal}>
					<ButtonText className="group gap-xs">
						<Icon
							className="stroke-2 stroke-tertiary text-transparent text-lg rotate-y-180 group-hover:stroke-green-light group-hover:text-green"
							icon="iconamoon:comment-fill"
						/>
						<TextSmall className="text-secondary">{formatNumber(post?.commentCount)}</TextSmall>
					</ButtonText>
				</a>
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

		{#if isLast}
			<Separator></Separator>
		{/if}
	</CardPrimary>
{/if}
