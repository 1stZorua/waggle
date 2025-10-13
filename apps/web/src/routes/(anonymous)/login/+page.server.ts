import { loginSchema } from '$lib/schemas';
import { fail, redirect, type Actions } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { AuthClient } from '@waggle/api-client/auth';
import { COOKIE_OPTIONS, MAX_AGE } from '$lib/config';
import type { PageServerLoad } from './$types';

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
				username: form.data.username,
				password: form.data.password
			});

			if (!data.accessToken || !data.refreshToken)
				return fail(401, { form, message: 'Invalid response from server' });

			cookies.set('access_token', data.accessToken, {
				...COOKIE_OPTIONS,
				maxAge: data.expiresIn
			});

			cookies.set('refresh_token', data.refreshToken, {
				...COOKIE_OPTIONS,
				maxAge: MAX_AGE
			});
		} catch (err) {
			console.error('Login error:', err);

			return fail(500, { form, message: 'Login failed' });
		}

		throw redirect(303, '/');
	}
};
