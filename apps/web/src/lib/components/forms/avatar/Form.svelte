<script lang="ts">
	import { Modal } from '$components/shared/other';
	import { EditAvatar, ActionsAvatar } from './steps';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { cn } from '$lib/utils';
	import { useAvatarModal } from '$lib/hooks/useAvatarModal.svelte';

	interface Props {
		data: LayoutData;
	}

	let { data }: Props = $props();

	const modal = useAvatarModal;

	const steps = {
		actions: { component: ActionsAvatar },
		edit: { component: EditAvatar }
	};

	const currentStep = $derived(steps[modal.mode]);

	function close() {
		modal.close();
	}
</script>

<Modal className={cn(currentStep === steps.actions ? 'p-0!' : '')} bind:isOpen={modal.isOpen}>
	<currentStep.component {data} avatar={modal.avatar!} onClose={close} />
</Modal>
