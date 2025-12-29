<script lang="ts">
	import { getFlash } from 'sveltekit-flash-message';
	import { page } from '$app/state';
	import { type Snippet } from 'svelte';
	import 'iconify-icon';
	import '../app.css';
	import { Toaster, toast } from 'svelte-sonner';
	import type { FlashType } from '$lib/types/types';

	const flash = getFlash(page);

	const toastMap: Record<FlashType, (message: string) => void> = {
		success: toast.success,
		info: toast.info,
		warning: toast.warning,
		error: toast.error
	};

	$effect(() => {
		if (!$flash) return;
		const toastFn = toastMap[$flash.type];
		toastFn($flash.message);
	});

	let { children }: { children?: Snippet } = $props();
</script>

<Toaster position="top-center" richColors />

{@render children?.()}
