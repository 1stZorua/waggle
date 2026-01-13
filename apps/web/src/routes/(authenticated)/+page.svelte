<script lang="ts">
	import { Button, ButtonAction, ButtonPrimary, ButtonText } from '$components/shared/buttons';
	import { Card, CardPrimary, CardSecondary } from '$components/shared/cards';
	import { Icon, Logo, InfiniteScroll, Avatar, TabControl } from '$components/shared/other';
	import { Post } from '$components/shared/posts';
	import { TextBase, TextSmall } from '$components/shared/text';
	import emblaCarouselSvelte from 'embla-carousel-svelte';
	import type { PageProps } from './$types';
	import { cn } from 'tailwind-variants';

	let { data }: PageProps = $props();

	let posts = $state(data.pageInfo.items);
	let nextCursor = $state(data.pageInfo.nextCursor);
	let loadingMore = $state(false);

	let emblaApi: any = $state();
	let selectedIndex = $state(0);

	const stories = ['Charlie', 'Chleo', 'Daisy', 'Luna', 'Bailey', 'Nibbs', 'Pip', 'Echo'];

	const onInit = (event: CustomEvent) => {
		emblaApi = event.detail;
		emblaApi.on('select', onSelect);
	};

	const onSelect = () => {
		if (!emblaApi) return;
		const newIndex = emblaApi.selectedScrollSnap();
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

	async function loadMore() {
		if (!nextCursor || loadingMore) return;

		loadingMore = true;

		try {
			const res = await fetch('/api/posts', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ cursor: nextCursor })
			});

			const result = await res.json();

			posts = [...posts, ...result.items];
			nextCursor = result.nextCursor;
		} finally {
			loadingMore = false;
		}
	}
</script>

<section
	class="gap-md relative grid h-full grid-cols-[minmax(0,1fr)_30rem] [grid-template-areas:'feed_action'] max-2xl:grid-cols-[1fr] max-2xl:[grid-template-areas:'feed']"
>
	<div class="gap-md flex h-full w-full min-w-0 flex-col">
		<div class="gap-md flex flex-col">
			<div class="flex justify-between">
				<TabControl items={['For you', 'Following']} changeTab={() => {}}></TabControl>
				<div class="gap-xs flex">
					<ButtonAction
						className={cn(
							'w-max rounded-md p-1 border-6 border-transparent',
							selectedIndex === 0
								? 'w-max'
								: 'border-pink-light bg-btn-primary-bg text-btn-primary-fg hover:bg-btn-primary-hover-bg hover:border-btn-primary-hover-border'
						)}
						onclick={scrollPrev}
					>
						<Icon icon="material-symbols:chevron-left-rounded"></Icon>
					</ButtonAction>
					<ButtonAction
						className={cn(
							'w-max rounded-md p-1 border-6 border-transparent',
							selectedIndex === stories.length / 2
								? 'w-max'
								: 'border-pink-light bg-btn-primary-bg text-btn-primary-fg hover:bg-btn-primary-hover-bg hover:border-btn-primary-hover-border'
						)}
						onclick={scrollNext}
					>
						<Icon icon="material-symbols:chevron-right-rounded"></Icon>
					</ButtonAction>
				</div>
			</div>
			<div class="w-full overflow-hidden">
				<div
					class="embla flex w-full"
					use:emblaCarouselSvelte={{
						options: {
							align: 'start',
							loop: false,
							dragFree: true,
							skipSnaps: false,
							containScroll: 'trimSnaps'
						},
						plugins: []
					}}
					onemblaInit={onInit}
				>
					<div class="embla__container gap-md flex w-full">
						<div class="embla__slide min-w-0 flex-[0_0_10.5rem]">
							<Card
								tag="a"
								href="#"
								className="p-0 rounded-xl overflow-hidden w-full aspect-2/3 h-60 relative text-background"
							>
								<div class="absolute h-full w-full bg-black/25"></div>
								<img class="h-full w-full object-cover blur-xs" src="/images/lila.webp" alt="pet" />
								<div class="gap-xs absolute bottom-4 flex w-full flex-col items-center">
									<ButtonText className="p-2 rounded-full border-background border-2">
										<Icon className="text-lg" icon="ic:round-plus"></Icon>
									</ButtonText>
									<TextSmall>Add Story</TextSmall>
								</div>
							</Card>
						</div>
						{#each stories as story, i}
							{@const pet = story.toLowerCase()}
							<div class="embla__slide min-w-0 flex-[0_0_10.5rem]">
								<Card
									tag="a"
									href="#"
									className="relative p-0 rounded-xl overflow-hidden w-full aspect-2/3 h-60 group"
								>
									<div class="absolute z-10 h-full w-full bg-black/10"></div>
									<img
										class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
										src="/images/{pet}.webp"
										alt={`Story ${i + 1}`}
									/>
									<div class="text-background absolute inset-4 flex flex-col justify-between">
										<Avatar
											className="border-background border-2"
											src="/images/{pet}.webp"
											alt={`Story ${i + 1}`}
										></Avatar>
										<TextSmall>{story}</TextSmall>
									</div>
								</Card>
							</div>
						{/each}
					</div>
				</div>
			</div>
		</div>

		{#if posts.length > 0}
			<div class="shadow-ui mx-auto flex max-w-[620px] flex-col rounded-lg">
				<InfiniteScroll
					items={posts}
					hasMore={!!nextCursor}
					isLoading={loadingMore}
					onLoadMore={loadMore}
				>
					{#snippet children({ items })}
						{#each items as post, index}
							<Post className="shadow-none!" {post} isLast={index < items.length - 1}></Post>
						{/each}
					{/snippet}

					{#snippet loadingContent()}
						<div class="py-4 text-center">
							<Icon className="text-secondary" icon="svg-spinners:90-ring-with-bg" />
						</div>
					{/snippet}
				</InfiniteScroll>

				{#if !nextCursor && !loadingMore}
					<div class="flex w-full items-center justify-center py-4">
						<Logo className="w-20 text-secondary"></Logo>
					</div>
				{/if}
			</div>
		{:else}
			<div class="gap-xs font-secondary flex h-full w-full flex-col items-center p-4">
				<Logo className="w-20 text-secondary"></Logo>
				<TextBase>Nothing fetched yet.</TextBase>
			</div>
		{/if}
	</div>

	<div class="action-bar gap-md fixed flex flex-col [grid-area:action] max-2xl:hidden">
		<CardPrimary className="h-max items-start flex-col">
			<div class="flex w-full items-center justify-between">
				<TextBase>Top Companions</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>
			<div class="gap-sm flex w-full flex-col">
				<div class="flex items-center justify-between">
					<div class="gap-sm flex items-center">
						<Avatar className="bg-accent" src="/images/dexter.webp" alt="pet"></Avatar>
						<div class="flex flex-col">
							<TextBase>Dexter</TextBase>
							<TextSmall className="text-secondary font-normal">Eyes on Mischief</TextSmall>
						</div>
					</div>
					<ButtonText>
						<Icon className="text-lg" icon="material-symbols:check-rounded"></Icon>
					</ButtonText>
				</div>
				<div class="flex items-center justify-between">
					<div class="gap-sm flex items-center">
						<Avatar className="bg-accent" src="/images/luna.webp" alt="pet"></Avatar>
						<div class="flex flex-col">
							<TextBase>Luna</TextBase>
							<TextSmall className="text-secondary font-normal">The Midnight Star</TextSmall>
						</div>
					</div>
					<ButtonText>
						<Icon className="text-secondary text-lg" icon="mingcute:user-add-line"></Icon>
					</ButtonText>
				</div>
				<div class="flex items-center justify-between">
					<div class="gap-sm flex items-center">
						<Avatar className="bg-accent" src="/images/echo.webp" alt="pet"></Avatar>
						<div class="flex flex-col">
							<TextBase>Echo</TextBase>
							<TextSmall className="text-secondary font-normal">The Voice of Joy</TextSmall>
						</div>
					</div>
					<ButtonText>
						<Icon className="text-secondary text-lg" icon="mingcute:user-add-line"></Icon>
					</ButtonText>
				</div>
			</div>
		</CardPrimary>
		<CardPrimary
			className="relative! items-start h-full pb-0 overflow-hidden flex-2 flex-col gap-sm min-h-0"
		>
			<div class="flex w-full items-center justify-between">
				<TextBase>Buddies</TextBase>
				<ButtonText>
					<TextSmall className="text-blue-dark">view all</TextSmall>
				</ButtonText>
			</div>

			<label class="bg-surface gap-xs flex w-full items-center rounded-md p-3">
				<Icon className="text-secondary text-lg" icon="uil:search" />
				<input
					class="w-full outline-none placeholder:font-medium"
					type="text"
					placeholder="Search"
				/>
			</label>

			<div
				class="gap-xl scrollbar-hidden flex w-full flex-1 flex-col overflow-x-hidden overflow-y-auto"
			>
				<div
					class="from-background/0 to-background pointer-events-none absolute bottom-0 left-0 h-12 w-full bg-linear-to-b"
				></div>
				<div class="gap-sm flex w-full flex-col">
					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/bailey.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Bailey</TextBase>
								<TextSmall className="text-secondary font-normal">The Loyal Friend</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/charlie.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Charlie</TextBase>
								<TextSmall className="text-secondary font-normal">Fetch Master</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/chleo.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Chleo</TextBase>
								<TextSmall className="text-secondary font-normal">The Snowy One</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/daisy.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Daisy</TextBase>
								<TextSmall className="text-secondary font-normal">Best Friend</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/dexter.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Dexter</TextBase>
								<TextSmall className="text-secondary font-normal">Eyes on Mischief</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center">
							<Avatar className="bg-accent" src="/images/nibbs.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Nibbs</TextBase>
								<TextSmall className="text-secondary font-normal">Little Explorer</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>

					<div class="flex items-center justify-between">
						<div class="gap-sm flex items-center pb-4">
							<Avatar className="bg-accent" src="/images/pip.webp" alt="pet" />
							<div class="flex flex-col">
								<TextBase>Pip</TextBase>
								<TextSmall className="text-secondary font-normal">Snack Hoarder</TextSmall>
							</div>
						</div>
						<ButtonText>
							<Icon className="text-secondary text-lg" icon="uis:ellipsis-h" />
						</ButtonText>
					</div>
				</div>
			</div>
		</CardPrimary>
	</div>
</section>
