import { loginSchema } from '$lib/schemas';
import { fail, redirect, type Actions } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { AuthClient } from '@waggle/api-client/auth';
import type { PageServerLoad } from './$types';
import { setAuthCookies } from '$lib/server';

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
			const { status, data, message } = await AuthClient.login({
				username: form.data.username,
				password: form.data.password
			}).then((res) => res.data);

			if (status !== 'success' || !data?.accessToken || !data?.refreshToken)
				return fail(401, { form, message: message ?? 'Invalid login credentials' });

			setAuthCookies(cookies, data.accessToken, data.refreshToken, data.expiresIn);
		} catch (err) {
			console.error(err);
			return fail(500, { form, message: 'Login failed' });
		}

		throw redirect(303, '/');
	}
};
