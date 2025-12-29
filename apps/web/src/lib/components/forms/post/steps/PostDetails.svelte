<script lang="ts">
	import { TextSmall } from '$components/shared/text';
	import type { CreatePostSchema } from '$lib/schemas';
	import type { SuperFormData, SuperFormErrors } from 'sveltekit-superforms/client';

	interface Props {
		form: SuperFormData<CreatePostSchema>;
		errors: SuperFormErrors<CreatePostSchema>;
		isOpen: boolean;
		onNext: () => void;
		onPrev: () => void;
		onClose: () => void;
	}

	let { form, errors, isOpen = $bindable(), onNext, onPrev, onClose }: Props = $props();
</script>

<TextSmall>Post Details</TextSmall>
