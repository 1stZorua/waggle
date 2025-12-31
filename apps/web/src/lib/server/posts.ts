import { PostClient } from '@waggle/api-client/post';
import { authHeaders } from './auth';
import { getCommentsByPostId } from './comments';
import { getUsersByIds } from './users';

export async function getPostsWithUsers(accessToken: string, cursor?: string) {
	try {
		const { data } = await PostClient.getAll(
			{ Cursor: cursor, PageSize: 1, Direction: 'forward' },
			{ headers: authHeaders(accessToken) }
		).then((res) => res.data);

		if (!data?.items || data.items.length === 0) {
			return { items: [], nextCursor: null };
		}

		const userIds = data.items.map((post) => post.userId) as string[];
		const userMap = await getUsersByIds(accessToken, userIds);

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
		const [postResponse, commentsResponse] = await Promise.allSettled([
			PostClient.getById(postId, { headers: authHeaders(accessToken) }).then((res) => res.data),
			getCommentsByPostId(accessToken, postId)
		]);

		if (postResponse.status === 'rejected' || !postResponse.value.data) {
			return null;
		}

		const post = postResponse.value.data;

		const comments =
			commentsResponse.status === 'fulfilled' && commentsResponse.value.items
				? commentsResponse.value.items
				: [];

		const commentsNextCursor =
			commentsResponse.status === 'fulfilled' ? commentsResponse.value.nextCursor : null;

		const userMap = await getUsersByIds(accessToken, [post.userId as string]);

		return {
			...post,
			user: userMap.get(post.userId as string) ?? null,
			comments,
			commentsNextCursor
		};
	} catch (err) {
		console.error(err);
		return null;
	}
}
