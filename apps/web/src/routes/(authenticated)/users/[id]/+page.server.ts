import type { PageServerLoad } from './$types';
import { getPostsByUserId } from '$lib/server/posts';
import { getUserById } from '$lib/server/users';

export const load: PageServerLoad = async ({ locals, params }) => {
	const userId = params.id;

	return {
		profile: await getUserById(locals.auth.accessToken, userId),
		userPosts: await getPostsByUserId(locals.auth.accessToken, userId)
	};
};
