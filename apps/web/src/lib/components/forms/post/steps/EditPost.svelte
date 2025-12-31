<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Dropdown, Icon, Separator } from '$components/shared/other';
	import { TextLarge, TextSmall } from '$components/shared/text';
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import type { Post } from '$lib/types/types';
	import { MAX_CAPTION } from '$lib/utils/constants';
	import { cn } from '$lib/utils';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';

	interface Props {
		data: LayoutData;
		post: Post | null;
		onClose: () => void;
	}

	let { data, post, onClose }: Props = $props();

	const initialCaption = post?.caption ?? '';

	const { form, errors, enhance } = superForm(data.updatePostForm, {
		onResult: ({ result }) => {
			if (result.type === 'success') {
				usePostModal.triggerSuccess();
				onClose();
			}
		},
		invalidateAll: false
	});

	let caption = $state(initialCaption);
	let charCount = $state(initialCaption.length);
	let textareaElement: HTMLTextAreaElement;
	let showEmojiPicker = $state(false);

	const emojis = [
		'😊',
		'😂',
		'❤️',
		'🔥',
		'✨',
		'👍',
		'🎉',
		'💯',
		'😍',
		'🥰',
		'😎',
		'🤩',
		'😢',
		'😭',
		'🙏',
		'💪'
	];

	function handleInput(e: Event) {
		const target = e.target as HTMLTextAreaElement;
		if (target.value.length > MAX_CAPTION) {
			target.value = target.value.slice(0, MAX_CAPTION);
		}
		caption = target.value;
		charCount = caption.length;
	}

	function handlePaste(e: ClipboardEvent) {
		const target = e.target as HTMLTextAreaElement;
		const pasteData = e.clipboardData?.getData('text') ?? '';
		if (target.value.length + pasteData.length > MAX_CAPTION) {
			e.preventDefault();
			const allowed = MAX_CAPTION - target.value.length;
			target.value += pasteData.slice(0, allowed);
			caption = target.value;
			charCount = caption.length;
		}
	}

	function insertEmoji(emoji: string) {
		const start = textareaElement.selectionStart;
		const end = textareaElement.selectionEnd;
		const before = caption.substring(0, start);
		const after = caption.substring(end);
		const newCaption = before + emoji + after;

		if (newCaption.length <= MAX_CAPTION) {
			caption = newCaption;
			charCount = caption.length;

			setTimeout(() => {
				textareaElement.focus();
				textareaElement.setSelectionRange(start + emoji.length, start + emoji.length);
			}, 0);
		}

		showEmojiPicker = false;
	}

	$effect(() => {
		$form.id = post?.id ?? '';
	});
</script>

<form class="gap-md flex h-125 w-200 flex-col" action="/posts?/update" method="POST" use:enhance>
	<input type="hidden" name="id" value={$form.id} />

	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Edit post</TextLarge>
		</div>
		<ButtonText type="button" onclick={onClose}>
			<Icon icon="material-symbols:close-rounded"></Icon>
		</ButtonText>
	</div>

	<Separator />

	<div class="gap-md flex min-h-0 flex-1 flex-col">
		<div class="gap-md flex min-h-0 flex-1">
			{#if post?.thumbnailId && post?.mediaUrls}
				<div class="min-h-0 flex-[2.5]">
					<img
						src={post.mediaUrls[post.thumbnailId].url}
						alt=""
						class="h-full w-full rounded-lg object-cover"
					/>
				</div>
			{/if}

			<div class="flex min-h-0 flex-2 flex-col gap-2">
				<textarea
					bind:this={textareaElement}
					class="shadow-ui h-full w-full resize-none rounded-lg p-4 outline-none"
					name="caption"
					placeholder="Say something about what's happening in this moment..."
					value={caption}
					oninput={handleInput}
					onpaste={handlePaste}
					required
				></textarea>

				{#if $errors.caption}
					<TextSmall className="text-pink">{$errors.caption}</TextSmall>
				{/if}

				<div class="flex items-center justify-between">
					<div class="relative">
						<ButtonText
							type="button"
							onclick={() => (showEmojiPicker = !showEmojiPicker)}
							className="text-xl"
						>
							<Icon icon="mdi:emoticon-outline"></Icon>
						</ButtonText>

						<Dropdown
							bind:show={showEmojiPicker}
							direction="bottom-left"
							width="w-72"
							className="p-3"
						>
							<div class="grid max-h-36 grid-cols-6 gap-2 overflow-x-hidden overflow-y-auto pr-2">
								{#each emojis as emoji}
									<ButtonText
										type="button"
										class="hover:bg-surface flex aspect-square cursor-pointer items-center justify-center rounded text-xl transition-colors"
										onclick={() => insertEmoji(emoji)}
									>
										{emoji}
									</ButtonText>
								{/each}
							</div>
						</Dropdown>
					</div>

					<TextSmall
						className={cn('font-normal', charCount >= MAX_CAPTION ? 'text-pink' : 'text-secondary')}
					>
						{charCount}/{MAX_CAPTION}
					</TextSmall>
				</div>
			</div>
		</div>

		<div class="gap-sm mt-2 flex justify-end">
			<ButtonAction type="button" className="rounded-lg" onclick={onClose}>Cancel</ButtonAction>
			<Button
				type="submit"
				props={{ variant: 'primary', size: 'md' }}
				className="rounded-lg w-max border-none"
				onclick={() => ($form.caption = caption)}
			>
				Save
			</Button>
		</div>
	</div>
</form>
