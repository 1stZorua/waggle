<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Icon, Separator } from '$components/shared/other';
	import { TextLarge, TextSmall } from '$components/shared/text';
	import type { CreatePostSchema } from '$lib/schemas';
	import type { SuperFormData, SuperFormErrors } from 'sveltekit-superforms/client';
	import { MAX_CAPTION } from '$lib/utils/constants';
	import { cn } from '$lib/utils/merge';

	interface Props {
		form: SuperFormData<CreatePostSchema>;
		errors: SuperFormErrors<CreatePostSchema>;
		isOpen: boolean;
		onNext: () => void;
		onPrev: () => void;
		onClose: () => void;
	}

	let { form, errors, isOpen = $bindable(), onNext, onPrev, onClose }: Props = $props();
	let caption = $state('');
	let charCount = $state(0);
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
</script>

<div class="gap-md flex h-125 w-200 flex-col">
	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Create new post</TextLarge>
		</div>
		<ButtonText onclick={onClose}>
			<Icon icon="material-symbols:close-rounded"></Icon>
		</ButtonText>
	</div>

	<Separator />

	<div class="gap-md flex min-h-0 flex-1 flex-col">
		<div class="gap-md flex min-h-0 flex-1">
			{#if $form.images.length > 0}
				<div class="min-h-0 flex-3">
					<img
						src={URL.createObjectURL($form.images[0])}
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

				<div class="flex items-center justify-between">
					<div class="relative">
						<ButtonText
							type="button"
							onclick={() => (showEmojiPicker = !showEmojiPicker)}
							className="text-xl"
						>
							<Icon icon="mdi:emoticon-outline"></Icon>
						</ButtonText>

						{#if showEmojiPicker}
							<div
								class="shadow-ui bg-background absolute top-full left-0 z-50 mt-2 w-72 overflow-hidden rounded-lg p-3"
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
							</div>
						{/if}
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
			<ButtonAction type="button" className="rounded-lg" onclick={onPrev}>Previous</ButtonAction>
			<Button
				type="submit"
				props={{ variant: 'primary', size: 'md' }}
				className="rounded-lg w-max border-none"
				onclick={() => ($form.caption = caption)}
			>
				Create
			</Button>
		</div>
	</div>
</div>
