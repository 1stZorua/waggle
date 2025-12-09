import type { PageServerLoad } from './$types';
import { PostClient } from '@waggle/api-client/post';
import { UserClient } from '@waggle/api-client/user';

async function getPostsWithUsers(authHeader: { Authorization: string }) {
	try {
		const postsResponse = await PostClient.getAll({}, { headers: authHeader });
		const posts = postsResponse.data.data;

		if (!posts?.items || posts.items.length === 0) return [];

		const userIds = [...new Set(posts.items.map((post) => post.userId))];

		if (userIds.length === 0) return posts.items.map((post) => ({ ...post, user: null }));

		const usersResponse = await UserClient.getByIds(
			{ ids: userIds as string[] },
			{ headers: authHeader }
		);

		if (!usersResponse.data.data) {
			return posts.items.map((post) => ({ ...post, user: null }));
		}

		const userMap = new Map(usersResponse.data.data.map((user) => [user.id, user]));

		return posts.items.map((post) => ({
			...post,
			user: userMap.get(post.userId) || null
		}));
	} catch (err) {
		console.error(err);
		return [];
	}
}

export const load: PageServerLoad = async ({ locals }) => {
	const authHeader = { Authorization: `Bearer ${locals.auth.accessToken}` };

	return {
		posts: getPostsWithUsers(authHeader)
	};
};
