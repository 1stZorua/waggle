<script lang="ts">
	import { superForm } from 'sveltekit-superforms';
	import type { LayoutData } from '../../../../../routes/(authenticated)/$types';
	import type { Post } from '$lib/types/types';
	import { UploadFiles, PostDetails } from './sub';
	import { goto } from '$app/navigation';

	interface Props {
		data: LayoutData;
		post: Post | null;
		onClose: () => void;
	}

	let { data, post, onClose }: Props = $props();

	const { form, errors, enhance, reset } = superForm(data.createPostForm, {
		onResult: async ({ result }) => {
			if (result.type === 'redirect') {
				onClose();
				location.href = '/';
			}
		}
	});

	const steps = [{ component: UploadFiles }, { component: PostDetails }];
	let currentStep = $state(0);

	function nextStep() {
		if (currentStep < steps.length - 1) currentStep += 1;
	}

	function prevStep() {
		if (currentStep > 0) currentStep -= 1;
	}

	function handleClose() {
		currentStep = 0;
		reset();
		onClose();
	}
</script>

<form
	class="flex h-full flex-col"
	action="/posts?/create"
	method="POST"
	enctype="multipart/form-data"
	use:enhance
>
	{#each steps as step, i}
		<div class="h-full" hidden={currentStep !== i}>
			<step.component {form} {errors} onNext={nextStep} onPrev={prevStep} onClose={handleClose} />
		</div>
	{/each}
</form>
