<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Dropdown, Icon, Separator } from '$components/shared/other';
	import { TextLarge, TextSmall } from '$components/shared/text';
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import type { Comment } from '$lib/types/types';
	import { MAX_COMMENT_LENGTH } from '$lib/utils/constants';
	import { cn } from '$lib/utils';
	import { useCommentModal } from '$lib/hooks/useCommentModal.svelte';

	interface Props {
		data: LayoutData;
		comment: Comment | null;
		onClose: () => void;
	}

	let { data, comment, onClose }: Props = $props();

	const initialContent = comment?.content ?? '';

	const { form, errors, enhance } = superForm(data.updateCommentForm, {
		onResult: ({ result }) => {
			if (result.type === 'success') {
				useCommentModal.onSuccess();
				onClose();
			}
		},
		invalidateAll: false
	});

	let content = $state(initialContent);
	let charCount = $state(initialContent.length);
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
		if (target.value.length > MAX_COMMENT_LENGTH) {
			target.value = target.value.slice(0, MAX_COMMENT_LENGTH);
		}
		content = target.value;
		charCount = content.length;
	}

	function handlePaste(e: ClipboardEvent) {
		const target = e.target as HTMLTextAreaElement;
		const pasteData = e.clipboardData?.getData('text') ?? '';
		if (target.value.length + pasteData.length > MAX_COMMENT_LENGTH) {
			e.preventDefault();
			const allowed = MAX_COMMENT_LENGTH - target.value.length;
			target.value += pasteData.slice(0, allowed);
			content = target.value;
			charCount = content.length;
		}
	}

	function insertEmoji(emoji: string) {
		const start = textareaElement.selectionStart;
		const end = textareaElement.selectionEnd;
		const before = content.substring(0, start);
		const after = content.substring(end);
		const newContent = before + emoji + after;

		if (newContent.length <= MAX_COMMENT_LENGTH) {
			content = newContent;
			charCount = content.length;

			setTimeout(() => {
				textareaElement.focus();
				textareaElement.setSelectionRange(start + emoji.length, start + emoji.length);
			}, 0);
		}

		showEmojiPicker = false;
	}

	$effect(() => {
		$form.id = comment?.id ?? '';
		$form.postId = comment?.postId ?? '';
	});
</script>

<form
	class="gap-md flex h-auto w-150 flex-col"
	action="/comments?/update"
	method="POST"
	use:enhance
>
	<input type="hidden" name="id" value={$form.id} />
	<input type="hidden" name="postId" value={$form.postId} />

	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Edit comment</TextLarge>
		</div>
		<ButtonText type="button" onclick={onClose}>
			<Icon icon="material-symbols:close-rounded"></Icon>
		</ButtonText>
	</div>

	<Separator />

	<div class="gap-md flex flex-col">
		<textarea
			bind:this={textareaElement}
			class="shadow-ui h-32 w-full resize-none rounded-lg p-4 outline-none"
			name="content"
			placeholder="Write a comment..."
			value={content}
			oninput={handleInput}
			onpaste={handlePaste}
			required
		></textarea>

		{#if $errors.content}
			<TextSmall className="text-pink">{$errors.content}</TextSmall>
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

				<Dropdown bind:show={showEmojiPicker} direction="bottom-left" width="w-72" className="p-3">
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
				className={cn(
					'font-normal',
					charCount >= MAX_COMMENT_LENGTH ? 'text-pink' : 'text-secondary'
				)}
			>
				{charCount}/{MAX_COMMENT_LENGTH}
			</TextSmall>
		</div>

		<div class="gap-sm mt-2 flex justify-end">
			<ButtonAction type="button" className="rounded-lg" onclick={onClose}>Cancel</ButtonAction>
			<Button
				type="submit"
				props={{ variant: 'primary', size: 'md' }}
				className="rounded-lg w-max border-none"
				onclick={() => ($form.content = content)}
			>
				Save
			</Button>
		</div>
	</div>
</form>
