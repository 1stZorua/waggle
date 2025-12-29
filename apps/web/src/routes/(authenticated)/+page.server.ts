import type { PageServerLoad } from './$types';
import { getPostsWithUsers } from '$lib/server/posts';

export const load: PageServerLoad = async ({ locals }) => {
	return {
		pageInfo: await getPostsWithUsers(locals.auth.accessToken)
	};
};
