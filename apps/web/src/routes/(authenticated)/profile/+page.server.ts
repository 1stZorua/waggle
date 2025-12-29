import type { PageServerLoad } from './$types';
import { getPostsByUserId } from '$lib/server/posts';

export const load: PageServerLoad = async ({ locals }) => {
	return { userPosts: await getPostsByUserId(locals.auth.accessToken, locals.user.id as string) };
};
