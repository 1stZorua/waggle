<script lang="ts">
	import { TextLarge } from '$components/shared/text';
</script>

<section class="gap-lg flex min-h-screen flex-col items-center justify-center">
	<div class="font-secondary flex flex-col items-center">
		<TextLarge className="text-9xl">404</TextLarge>
		<TextLarge>Page Not Found</TextLarge>
		<TextLarge className="text-secondary font-normal"
			>Looks like this page went for a walk.</TextLarge
		>
	</div>
	<img class="absolute right-0 bottom-0 w-1/3 object-cover" src="images/pet_404.png" alt="dog" />
</section>
