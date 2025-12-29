import type { PageServerLoad } from './$types';
import { getPostById } from '$lib/server/posts';

export const load: PageServerLoad = async ({ locals, params }) => {
	const { id } = params;
	return { post: getPostById(locals.auth.accessToken, id) };
};
