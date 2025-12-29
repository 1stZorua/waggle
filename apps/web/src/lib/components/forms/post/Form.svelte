<script lang="ts">
	import { ButtonPrimary, ButtonText } from '$components/shared/buttons';
	import { Modal } from '$components/shared/other';
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';
	import { UploadFiles, AssignPets, PostDetails } from './steps';

	const steps = [{ component: UploadFiles }, { component: AssignPets }, { component: PostDetails }];

	interface Props {
		isOpen: boolean;
		data: LayoutData;
	}

	let { isOpen = $bindable(), data }: Props = $props();
	const { form, errors, enhance, reset } = superForm(data.createPostForm, {
		onResult: ({ result }) => {
			if (result.type === 'redirect') close();
		}
	});

	let currentStep = $state(0);

	function nextStep() {
		if (currentStep < steps.length - 1) currentStep += 1;
	}

	function prevStep() {
		if (currentStep > 0) currentStep -= 1;
	}

	function close() {
		isOpen = false;
		currentStep = 0;
		reset();
	}
</script>

<Modal bind:isOpen>
	<form
		class="flex h-full flex-col"
		action="/posts?/create"
		method="POST"
		enctype="multipart/form-data"
		use:enhance
	>
		{#each steps as step, i}
			<div class="h-full" hidden={currentStep !== i}>
				<step.component {form} {errors} onNext={nextStep} onPrev={prevStep} onClose={close} />
			</div>
		{/each}

		{#if currentStep === steps.length - 1}
			<ButtonPrimary type="submit">Submit</ButtonPrimary>
		{/if}
	</form>
</Modal>
