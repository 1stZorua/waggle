import { redirect } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import { createPostSchema } from '$lib/schemas';

export const load: LayoutServerLoad = async ({ locals }) => {
	if (!locals.auth) throw redirect(302, '/login');

	const createPostForm = await superValidate(zod4(createPostSchema));

	return {
		user: locals.user,
		createPostForm: createPostForm
	};
};
