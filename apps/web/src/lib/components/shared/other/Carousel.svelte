<script lang="ts">
	import { Icon } from '$components/shared/other';
	import { cn } from '$lib/utils';
	import emblaCarouselSvelte from 'embla-carousel-svelte';
	import { TextSmall } from '../text';

	interface Props {
		images: string[];
		className?: string;
		imageClassName?: string;
	}

	let { images, className = '', imageClassName = '' }: Props = $props();

	let emblaApi: any = $state();
	let selectedIndex = $state(0);
	let hasScrolledForward = $state(false);

	const onInit = (event: CustomEvent) => {
		emblaApi = event.detail;
		emblaApi.on('select', onSelect);
	};

	const onSelect = () => {
		if (!emblaApi) return;
		const newIndex = emblaApi.selectedScrollSnap();

		if (newIndex > selectedIndex) {
			hasScrolledForward = true;
		}

		selectedIndex = newIndex;
	};

	const scrollPrev = (e: MouseEvent) => {
		e.preventDefault();
		e.stopPropagation();
		emblaApi?.scrollPrev();
	};
	const scrollNext = (e: MouseEvent) => {
		e.preventDefault();
		e.stopPropagation();
		emblaApi?.scrollNext();
	};
</script>

<div class={cn('relative h-full w-full', className)}>
	<div
		class="embla h-full overflow-hidden"
		use:emblaCarouselSvelte={{
			options: {
				loop: true,
				align: 'start',
				duration: 0,
				skipSnaps: false
			},
			plugins: []
		}}
		onemblaInit={onInit}
	>
		<div class="embla__container flex h-full">
			{#each images as image, i}
				<div class="embla__slide relative h-full min-w-0 flex-[0_0_100%]">
					<img src={image} alt={`Slide ${i + 1}`} class={cn(imageClassName)} />
				</div>
			{/each}
		</div>
	</div>

	{#if images.length > 1}
		<div
			class="text-background absolute top-4 right-4 z-10 flex items-center justify-center rounded-full bg-black/50 px-3 py-1.5"
		>
			<TextSmall className="text-xs">{selectedIndex + 1}/{images.length}</TextSmall>
		</div>

		{#if hasScrolledForward}
			<button
				type="button"
				class="absolute top-1/2 left-4 z-10 flex h-8 w-8 -translate-y-1/2 cursor-pointer items-center justify-center rounded-full bg-black/50 text-white transition-colors hover:bg-black/70"
				onclick={scrollPrev}
				aria-label="Previous image"
			>
				<Icon icon="material-symbols:arrow-back-ios-new-rounded" className="text-sm" />
			</button>
		{/if}

		<button
			type="button"
			class="absolute top-1/2 right-4 z-10 flex h-8 w-8 -translate-y-1/2 cursor-pointer items-center justify-center rounded-full bg-black/50 text-white transition-colors hover:bg-black/70"
			onclick={scrollNext}
			aria-label="Next image"
		>
			<Icon icon="material-symbols:arrow-forward-ios-rounded" className="text-sm" />
		</button>
	{/if}
</div>
