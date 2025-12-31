<script lang="ts">
	import { Button, ButtonAction, ButtonText } from '$components/shared/buttons';
	import { Icon, Separator } from '$components/shared/other';
	import { TextBase, TextLarge, TextSmall } from '$components/shared/text';
	import { cn } from '$lib/utils/merge';
	import { getFlash } from 'sveltekit-flash-message';
	import { page } from '$app/state';
	import type { User } from '@waggle/api-client/user/model';
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import { invalidateAll } from '$app/navigation';

	interface Props {
		data: LayoutData;
		avatar: User | null;
		onClose: () => void;
	}

	const flash = getFlash(page);

	let { data, avatar, onClose }: Props = $props();

	const { form, errors, enhance } = superForm(data.updateUserForm, {
		onResult: ({ result }) => {
			if (result.type === 'success') {
				onClose();
				invalidateAll();
			}
		},
		invalidateAll: true
	});

	let fileInput: HTMLInputElement | undefined = $state();
	let isDragging = $state(false);

	let uploadedFile: { file: File; url: string } | null = $state(null);

	function syncInputFile() {
		if (!fileInput) return;

		if (uploadedFile) {
			const dt = new DataTransfer();
			dt.items.add(uploadedFile.file);
			fileInput.files = dt.files;
		} else {
			fileInput.value = '';
		}
	}

	function handleFile(files: FileList | null) {
		if (!files || files.length === 0) return;

		const file = files[0];
		if (!file.type.startsWith('image/')) {
			$flash = { type: 'error', message: 'Please select an image file' };
			return;
		}

		if (uploadedFile) {
			URL.revokeObjectURL(uploadedFile.url);
		}

		const url = URL.createObjectURL(file);
		uploadedFile = { file, url };
	}

	function uploadImage(e: Event) {
		handleFile((e.target as HTMLInputElement).files);
	}

	function onDrop(e: DragEvent) {
		e.preventDefault();
		isDragging = false;
		handleFile(e.dataTransfer?.files ?? null);
	}

	function onKeydown(e: KeyboardEvent) {
		if (e.key === 'Enter' || e.key === ' ') {
			e.preventDefault();
			fileInput?.click();
		}
	}

	function removeFile() {
		if (uploadedFile) {
			URL.revokeObjectURL(uploadedFile.url);
			uploadedFile = null;
		}

		if (fileInput) {
			fileInput.value = '';
		}
	}

	$effect(() => {
		$form.id = avatar?.id ?? '';
		if (uploadedFile !== null) $form.avatar = uploadedFile.file;
		syncInputFile();
	});

	$effect(() => {
		return () => {
			if (uploadedFile) {
				URL.revokeObjectURL(uploadedFile.url);
			}
		};
	});
</script>

<form
	class="gap-md flex h-125 w-200 flex-col"
	action="/users?/update"
	method="POST"
	use:enhance
	enctype="multipart/form-data"
>
	<input type="hidden" name="id" value={$form.id} />

	<div class="gap-xl flex items-center justify-between">
		<div class="flex flex-col">
			<TextLarge>Edit avatar</TextLarge>
			<TextSmall className="text-secondary font-normal">Upload a new profile picture</TextSmall>
		</div>
		<ButtonText onclick={onClose}>
			<Icon icon="material-symbols:close-rounded" />
		</ButtonText>
	</div>

	<Separator />

	{#if uploadedFile || avatar?.avatarUrl}
		<div class="flex flex-1 flex-col items-center justify-center gap-4">
			<div class="group relative">
				<button
					type="button"
					class="focus:ring-primary relative cursor-pointer overflow-hidden rounded-full"
					onclick={() => fileInput?.click()}
				>
					<img
						src={uploadedFile?.url ?? avatar?.avatarUrl?.url}
						alt="avatar"
						class="h-64 w-64 rounded-full object-cover shadow-lg transition-opacity"
					/>
					<div
						class="absolute inset-0 flex items-center justify-center bg-black/50 opacity-0 transition-opacity duration-500 group-hover:opacity-100"
					>
						<div class="flex flex-col items-center gap-2 text-white">
							<Icon icon="iconoir:cloud-upload" className="text-2xl" />
							<TextSmall className="text-white">Change avatar</TextSmall>
						</div>
					</div>
				</button>

				{#if uploadedFile}
					<ButtonText
						className="absolute top-2 right-2 flex h-8 w-8 items-center justify-center rounded-full bg-black/50 text-white hover:bg-black/70 transition-colors"
						onclick={removeFile}
					>
						<Icon icon="material-symbols:close-rounded" />
					</ButtonText>
				{/if}
			</div>

			{#if $errors.avatar}
				<TextSmall className="text-pink">{$errors.avatar}</TextSmall>
			{/if}
		</div>

		<div class="gap-sm mt-auto flex justify-end">
			<ButtonAction type="button" className="rounded-lg" onclick={onClose}>Cancel</ButtonAction>
			<Button
				type="submit"
				props={{ variant: 'primary', size: 'md' }}
				className="w-max rounded-lg border-none"
				disabled={!uploadedFile}
			>
				Save
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
		name="avatar"
		type="file"
		accept="image/*"
		onchange={uploadImage}
		hidden
	/>
</form>
