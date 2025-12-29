import { authHeaders, deleteAuthCookies } from '$lib/server';
import { AuthClient } from '@waggle/api-client/auth';
import { type Actions } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';
import { redirect } from 'sveltekit-flash-message/server';
import { superValidate } from 'sveltekit-superforms';
import { zod4 } from 'sveltekit-superforms/adapters';
import { deleteUserSchema } from '$lib/schemas';
import { handleFormAction } from '$lib/server';

export const load: PageServerLoad = async () => {
	const form = await superValidate(zod4(deleteUserSchema));

	return { form };
};

export const actions: Actions = {
	delete: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(deleteUserSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await AuthClient.delete({ id: data.id }, { headers: authHeaders(locals.auth.accessToken) });
			},
			cookies
		);

		if (error) return error;

		deleteAuthCookies(cookies);
		throw redirect('/login', { type: 'success', message: 'Successfully deleted account' }, cookies);
	}
};
