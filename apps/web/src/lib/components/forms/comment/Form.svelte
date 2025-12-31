<script lang="ts">
	import { Modal } from '$components/shared/other';
	import { CreateComment, EditComment, DeleteComment, ActionsComment } from './steps';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { useCommentModal } from '$lib/hooks/useCommentModal.svelte';
	import { cn } from '$lib/utils';

	interface Props {
		data: LayoutData;
	}

	let { data }: Props = $props();

	const modal = useCommentModal;

	const steps = {
		actions: { component: ActionsComment },
		create: { component: CreateComment },
		edit: { component: EditComment },
		delete: { component: DeleteComment }
	};

	const currentStep = $derived(steps[modal.mode]);

	function close() {
		modal.close();
	}
</script>

<Modal className={cn(currentStep === steps.actions ? 'p-0!' : '')} bind:isOpen={modal.isOpen}>
	<currentStep.component {data} comment={modal.comment!} onClose={close} />
</Modal>
