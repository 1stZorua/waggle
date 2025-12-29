import { PostClient } from '@waggle/api-client/post';
import { UserClient } from '@waggle/api-client/user';
import { authHeaders } from './auth';

export async function getPostsWithUsers(accessToken: string, cursor?: string) {
	try {
		const { data } = await PostClient.getAll(
			{ Cursor: cursor, PageSize: 1, Direction: 'forward' },
			{ headers: authHeaders(accessToken) }
		).then((res) => res.data);

		if (!data?.items || data.items.length === 0) {
			return { items: [], nextCursor: null };
		}

		const userIds = [...new Set(data.items.map((post) => post.userId))];

		if (userIds.length === 0) {
			return {
				items: data.items.map((post) => ({ ...post, user: null })),
				nextCursor: data.pageInfo?.nextCursor ?? null
			};
		}

		const usersResponse = await UserClient.getByIds(
			{ ids: userIds as string[] },
			{ headers: authHeaders(accessToken) }
		);

		const userMap = new Map((usersResponse.data.data || []).map((user) => [user.id, user]));

		return {
			items: data.items.map((post) => ({
				...post,
				user: userMap.get(post.userId) ?? null
			})),
			nextCursor: data.pageInfo?.nextCursor ?? null
		};
	} catch (err) {
		console.error(err);
		return { items: [], nextCursor: null };
	}
}

export async function getPostsByUserId(accessToken: string, userId: string, cursor?: string) {
	try {
		const { data } = await PostClient.getAllByUserId(
			userId,
			{
				Cursor: cursor,
				PageSize: 12,
				Direction: 'forward'
			},
			{ headers: authHeaders(accessToken) }
		).then((res) => res.data);

		if (!data?.items || data.items.length === 0) return { items: [], nextCursor: null };

		return {
			items: data.items,
			nextCursor: data.pageInfo?.nextCursor ?? null
		};
	} catch (err) {
		console.error(err);
		return { items: [], nextCursor: null };
	}
}

export async function getPostById(accessToken: string, postId: string) {
	try {
		const { data: post } = await PostClient.getById(postId, {
			headers: authHeaders(accessToken)
		}).then((res) => res.data);
		if (!post) return null;

		let user = null;
		if (post.userId) {
			const { data } = await UserClient.getById(post.userId, {
				headers: authHeaders(accessToken)
			}).then((res) => res.data);

			user = data;
		}

		return { ...post, user };
	} catch (err) {
		console.error(err);
		return null;
	}
}
