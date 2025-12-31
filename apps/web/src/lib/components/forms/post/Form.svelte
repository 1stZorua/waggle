<script lang="ts">
	import { Modal } from '$components/shared/other';
	import { CreatePost, EditPost, DeletePost, ActionsPost } from './steps';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { usePostModal } from '$lib/hooks/usePostModal.svelte';
	import { cn } from '$lib/utils';

	interface Props {
		data: LayoutData;
	}

	let { data }: Props = $props();

	const modal = usePostModal;

	const steps = {
		actions: { component: ActionsPost },
		create: { component: CreatePost },
		edit: { component: EditPost },
		delete: { component: DeletePost }
	};

	const currentStep = $derived(steps[modal.mode]);

	function close() {
		modal.close();
	}
</script>

<Modal className={cn(currentStep === steps.actions ? 'p-0!' : '')} bind:isOpen={modal.isOpen}>
	<currentStep.component {data} post={modal.post!} onClose={close} />
</Modal>
