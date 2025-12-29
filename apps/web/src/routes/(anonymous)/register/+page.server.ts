import { registerSchema } from '$lib/schemas';
import { type Actions } from '@sveltejs/kit';
import { redirect } from 'sveltekit-flash-message/server';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { AuthClient } from '@waggle/api-client/auth';
import type { PageServerLoad } from './$types';
import { handleFormAction } from '$lib/server';

export const load: PageServerLoad = async () => {
	const form = await superValidate(zod4(registerSchema));
	return { form };
};

export const actions: Actions = {
	register: async ({ request, cookies }) => {
		const form = await superValidate(request, zod4(registerSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await AuthClient.register({
					username: data.username,
					email: data.email,
					firstName: data.firstName,
					lastName: data.lastName,
					password: data.password,
					confirmPassword: data.confirmPassword
				});
			},
			cookies
		);

		if (error) return error;

		redirect('/login', { type: 'success', message: 'Successfully created account' }, cookies);
	}
};
