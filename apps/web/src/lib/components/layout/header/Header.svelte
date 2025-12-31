<script lang="ts">
	import type { ClassType } from '$components/_types';
	import { ButtonText } from '$components/shared/buttons';
	import { Avatar, Icon, Logo } from '$components/shared/other';
	import { cn } from 'tailwind-variants';
	import type { LayoutData } from '../../../../routes/(authenticated)/$types';

	let { className, data }: { className: ClassType; data: LayoutData } = $props();
</script>

<header
	class={cn(
		'h-header wrapper pointer-events-none fixed top-0 left-0 z-10 flex w-full items-center justify-between',
		className
	)}
>
	<ButtonText href="/" class="pointer-events-auto">
		<Logo className="w-20"></Logo>
	</ButtonText>

	<div class="gap-sm flex items-center">
		<div class="pointer-events-auto relative">
			<ButtonText>
				<Icon icon="mingcute:notification-line" />
				<span class="bg-pink absolute top-0.5 right-1 h-2 w-2 rounded-full"></span>
			</ButtonText>
		</div>
		<ButtonText href="/profile" class="pointer-events-auto">
			<Avatar src={data.profile?.avatarUrl?.url ?? '/images/anonymous.png'} alt="avatar"></Avatar>
		</ButtonText>
	</div>
</header>
