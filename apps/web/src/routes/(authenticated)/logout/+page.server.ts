import { deleteAuthCookies } from '$lib/server';
import type { Actions, PageServerLoad } from './$types';
import { redirect } from '@sveltejs/kit';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	default: async ({ cookies }) => {
		deleteAuthCookies(cookies);
		throw redirect(302, '/login');
	}
};
