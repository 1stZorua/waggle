import { loginSchema } from '$lib/schemas';
import { fail, type Actions } from '@sveltejs/kit';
import { redirect, setFlash } from 'sveltekit-flash-message/server';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { AuthClient } from '@waggle/api-client/auth';
import type { PageServerLoad } from './$types';
import { setAuthCookies } from '$lib/server';
import { isApiError } from '@waggle/api-client';

export const load: PageServerLoad = async ({ locals }) => {
	if (locals.auth) throw redirect(302, '/');

	const form = await superValidate(zod4(loginSchema));
	return { form };
};

export const actions: Actions = {
	login: async ({ request, cookies }) => {
		const form = await superValidate(request, zod4(loginSchema));

		if (!form.valid) return fail(400, { form });

		try {
			const { data } = await AuthClient.login({
				identifier: form.data.identifier,
				password: form.data.password
			}).then((res) => res.data);

			if (!data?.accessToken || !data?.refreshToken) return;

			setAuthCookies(cookies, data.accessToken, data.refreshToken, data.expiresIn);
		} catch (err) {
			let message = 'Login service is temporarily unavailable';
			if (isApiError(err)) message = err.body?.message ?? message;

			console.error(err);
			setFlash({ type: 'error', message }, cookies);
			return fail(400, { form, message });
		}

		redirect('/', { type: 'success', message: 'Successfully logged in' }, cookies);
	}
};
