<script lang="ts">
	import { Modal } from '$components/shared/other';
	import { DeleteUser, ActionsUser } from './steps';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { cn } from '$lib/utils';
	import { useUserModal } from '$lib/hooks/useUserModal.svelte';

	interface Props {
		data: LayoutData;
	}

	let { data }: Props = $props();

	const modal = useUserModal;

	const steps = {
		actions: { component: ActionsUser },
		delete: { component: DeleteUser }
	};

	const currentStep = $derived(steps[modal.mode]);

	function close() {
		modal.close();
	}
</script>

<Modal className={cn(currentStep === steps.actions ? 'p-0!' : '')} bind:isOpen={modal.isOpen}>
	<currentStep.component {data} profile={modal.profile!} onClose={close} />
</Modal>
