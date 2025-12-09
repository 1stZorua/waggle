<script lang="ts">
	import { ButtonText } from '$components/shared/buttons';
	import { Avatar, Icon } from '$components/shared/other';
</script>

<header class="wrapper h-header fixed top-0 right-0 left-0 flex items-center justify-between">
	<ButtonText href="/" className="w-full">
		<img class="w-20" src="images/logo.svg" alt="Waggle" />
	</ButtonText>
	<div class="gap-sm flex items-center">
		<div class="relative">
			<ButtonText>
				<Icon icon="mingcute:notification-line" />
				<span class="bg-pink absolute top-0.5 right-1 h-2 w-2 rounded-full"></span>
			</ButtonText>
		</div>
		<ButtonText>
			<Avatar src="images/avatar.png" alt="avatar"></Avatar>
		</ButtonText>
	</div>
</header>
