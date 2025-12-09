<script lang="ts">
	import { cn } from '$lib/utils/merge';
	import type { ClassType } from '$components/_types';
	import type { HTMLInputAttributes } from 'svelte/elements';
	import type { Snippet } from 'svelte';
	import { TextSmall } from '../text';
	import { slide } from 'svelte/transition';
	import { quintOut } from 'svelte/easing';
	import Icon from './Icon.svelte';

	interface Props extends HTMLInputAttributes {
		className?: ClassType;
		errors?: string[];
		children?: Snippet;
	}

	let { className, errors, children, ...rest }: Props = $props();
</script>

<div class="gap-sm flex flex-col">
	<div
		class={cn(
			'shadow-ui focus-within:ring-accent relative flex items-center rounded-md px-8 py-3 focus-within:ring-2',
			errors?.length && 'border-pink-light border-3',
			className
		)}
	>
		<input
			class="placeholder:text-secondary w-full py-2 pr-5 pl-3 text-base transition-opacity outline-none"
			{...rest}
		/>
		{@render children?.()}
		{#if errors?.length}
			<Icon className="absolute right-6 text-md text-pink" icon="material-symbols:error-rounded" />
		{/if}
	</div>
	{#if errors?.length}
		<ul transition:slide={{ duration: 300, easing: quintOut }}>
			{#each errors as err (err)}
				<li>
					<TextSmall className="font-normal text-pink">{err}</TextSmall>
				</li>
			{/each}
		</ul>
	{/if}
</div>
