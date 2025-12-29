import { loginSchema } from '$lib/schemas';
import { type Actions } from '@sveltejs/kit';
import { redirect } from 'sveltekit-flash-message/server';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { AuthClient } from '@waggle/api-client/auth';
import type { PageServerLoad } from './$types';
import { setAuthCookies } from '$lib/server';
import { handleFormAction } from '$lib/server';

export const load: PageServerLoad = async () => {
	const form = await superValidate(zod4(loginSchema));
	return { form };
};

export const actions: Actions = {
	login: async ({ request, cookies }) => {
		const form = await superValidate(request, zod4(loginSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				const { data: authData } = await AuthClient.login({
					identifier: data.identifier,
					password: data.password
				}).then((res) => res.data);

				if (!authData?.accessToken || !authData?.refreshToken) {
					throw new Error('Invalid auth response');
				}

				setAuthCookies(cookies, authData.accessToken, authData.refreshToken, authData.expiresIn);
			},
			cookies
		);

		if (error) return error;

		redirect('/', { type: 'success', message: 'Successfully logged in' }, cookies);
	}
};
