<script lang="ts">
	import { superForm } from 'sveltekit-superforms';

	let { data } = $props();

	const { form, enhance } = superForm(data.form);
</script>

<form action="?/login" method="POST" use:enhance>
	<input required type="text" name="username" value={$form.username} placeholder="Username" />
	<input required type="password" name="password" value={$form.password} placeholder="Password" />
	<button type="submit">Login</button>
</form>
