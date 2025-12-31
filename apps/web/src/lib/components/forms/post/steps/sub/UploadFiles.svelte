<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Icon, Separator } from '$components/shared/other';
	import { TextBase, TextLarge, TextSmall } from '$components/shared/text';
	import type { CreatePostSchema } from '$lib/schemas';
	import { MAX_IMAGES } from '$lib/utils/constants';
	import { cn } from '$lib/utils/merge';
	import { getFlash } from 'sveltekit-flash-message';
	import type { SuperFormData, SuperFormErrors } from 'sveltekit-superforms/client';
	import { page } from '$app/state';

	interface Props {
		form: SuperFormData<CreatePostSchema>;
		errors: SuperFormErrors<CreatePostSchema>;
		onNext: () => void;
		onPrev: () => void;
		onClose: () => void;
	}

	const flash = getFlash(page);

	let { form, errors, onNext, onPrev, onClose }: Props = $props();

	let fileInput: HTMLInputElement | undefined = $state();
	let isDragging = $state(false);

	let uploadedFiles: { file: File; url: string }[] = $state([]);
	let dragIndex: number | null = null;

	function syncInputFiles() {
		if (!fileInput) return;

		const dt = new DataTransfer();
		for (const { file } of uploadedFiles) {
			dt.items.add(file);
		}

		fileInput.files = dt.files;
	}

	function handleFiles(files: FileList | null) {
		if (!files || files.length === 0) return;

		for (const file of Array.from(files)) {
			if (uploadedFiles.length >= MAX_IMAGES) {
				$flash = { type: 'error', message: `Maximum of ${MAX_IMAGES} files allowed` };
				break;
			}

			const url = URL.createObjectURL(file);
			uploadedFiles = [...uploadedFiles, { file, url }];
		}
	}

	function uploadImage(e: Event) {
		handleFiles((e.target as HTMLInputElement).files);
	}

	function onDrop(e: DragEvent) {
		e.preventDefault();
		isDragging = false;
		handleFiles(e.dataTransfer?.files ?? null);
	}

	function onKeydown(e: KeyboardEvent) {
		if (e.key === 'Enter' || e.key === ' ') {
			e.preventDefault();
			fileInput?.click();
		}
	}

	function removeFile(index: number) {
		const removed = uploadedFiles[index];
		URL.revokeObjectURL(removed.url);

		uploadedFiles = uploadedFiles.filter((_, i) => i !== index);

		if (uploadedFiles.length === 0 && fileInput) {
			fileInput.value = '';
		}
	}

	function dragStart(index: number) {
		dragIndex = index;
	}

	function dragOver(e: DragEvent) {
		e.preventDefault();
	}

	function drop(index: number) {
		if (dragIndex === null) return;

		const items = [...uploadedFiles];
		const [moved] = items.splice(dragIndex, 1);
		items.splice(index, 0, moved);

		uploadedFiles = items;
		dragIndex = null;
	}

	$effect(() => {
		$form.images = uploadedFiles.map((f) => f.file);
		syncInputFiles();
	});
</script>

<div class="gap-md flex h-125 w-200 flex-col">
	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Create new post</TextLarge>
			<TextSmall className="text-secondary font-normal">
				Select and upload the files of your choice
			</TextSmall>
		</div>
		<ButtonText onclick={onClose}>
			<Icon icon="material-symbols:close-rounded" />
		</ButtonText>
	</div>

	<Separator />

	{#if uploadedFiles.length > 0}
		<div class="gap-sm flex flex-wrap">
			{#each uploadedFiles as fileData, index (fileData.url)}
				<div
					class="relative cursor-move overflow-hidden rounded-md"
					role="listitem"
					draggable="true"
					ondragstart={() => dragStart(index)}
					ondragover={dragOver}
					ondrop={() => drop(index)}
				>
					<img src={fileData.url} alt={fileData.file.name} class="h-24 w-24 object-cover" />

					{#if index === 0}
						<div
							class="absolute bottom-1 left-1/2 -translate-x-1/2 rounded bg-black/60 px-1.5 py-0.5 text-xs text-white"
						>
							Thumbnail
						</div>
					{/if}

					<ButtonText
						className="absolute top-1 right-1 flex h-5 w-5 items-center justify-center rounded-full bg-black/50 text-white"
						onclick={() => removeFile(index)}
					>
						<TextSmall>&times;</TextSmall>
					</ButtonText>
				</div>
			{/each}

			{#if uploadedFiles.length < MAX_IMAGES}
				<ButtonText
					type="button"
					className="flex h-24 w-24 items-center justify-center rounded-lg shadow-ui"
					onclick={() => fileInput?.click()}
				>
					<Icon icon="ic:round-plus" />
				</ButtonText>
			{/if}
		</div>

		<div class="gap-sm mt-auto flex justify-end">
			<ButtonAction type="button" className="rounded-lg" onclick={onClose}>Cancel</ButtonAction>
			<Button
				type="button"
				props={{ variant: 'primary', size: 'md' }}
				className="w-max rounded-lg border-none"
				onclick={onNext}
			>
				Continue
			</Button>
		</div>
	{:else}
		<div
			class={cn(
				'gap-md flex h-full cursor-pointer flex-col items-center justify-center transition-colors duration-500 outline-none',
				isDragging ? 'bg-surface' : ''
			)}
			style="
				background-image: url('data:image/svg+xml,%3csvg width=\'100%25\' height=\'100%25\' xmlns=\'http://www.w3.org/2000/svg\'%3e%3crect width=\'100%25\' height=\'100%25\' fill=\'none\' rx=\'10\' ry=\'10\' stroke=\'%23e5e3db\' stroke-width=\'3\' stroke-dasharray=\'18%2c 13\' stroke-dashoffset=\'116\' stroke-linecap=\'round\'/%3e%3c/svg%3e');
				border-radius: 10px;
			"
			role="button"
			tabindex="0"
			onclick={() => fileInput?.click()}
			ondragover={(e) => e.preventDefault()}
			ondragenter={() => (isDragging = true)}
			ondragleave={() => (isDragging = false)}
			ondrop={onDrop}
			onkeydown={onKeydown}
		>
			<Icon icon="iconoir:cloud-upload" />
			<div class="flex flex-col items-center">
				<TextBase className="text-primary">Choose a file or drag & drop it here.</TextBase>
				<TextBase className="font-normal text-secondary">
					PNG, JPEG, WEBP and AVIF formats, up to 10MB.
				</TextBase>
			</div>
			<ButtonAction type="button" className="flex gap-sm rounded-lg">
				<TextBase>Browse Files</TextBase>
			</ButtonAction>
		</div>
	{/if}

	<input
		bind:this={fileInput}
		name="images"
		type="file"
		multiple
		accept="image/*"
		onchange={uploadImage}
		hidden
	/>
</div>
