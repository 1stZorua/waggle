<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { Separator } from '$components/shared/other';
	import { TextBase } from '$components/shared/text';
	import { useAvatarModal } from '$lib/hooks/useAvatarModal.svelte';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import type { User } from '@waggle/api-client/user/model';

	interface Props {
		data: LayoutData;
		avatar: User | null;
		onClose: () => void;
	}
	let { data, avatar, onClose }: Props = $props();

	function handleEdit() {
		useAvatarModal.setMode('edit');
	}
</script>

<div class="gap-md flex h-auto w-150 flex-col">
	<div class="flex flex-col">
		<ButtonText onclick={handleEdit} className="justify-center p-4 w-full">
			<TextBase>Upload Photo</TextBase>
		</ButtonText>
		<Separator></Separator>
		<ButtonText onclick={onClose} className="justify-center p-4 w-full">
			<TextBase>Cancel</TextBase>
		</ButtonText>
	</div>
</div>
