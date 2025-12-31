import { CommentClient } from '@waggle/api-client/comment';
import { authHeaders } from './auth';
import { getUsersByIds } from './users';

export async function getCommentsByPostId(accessToken: string, postId: string, cursor?: string) {
	try {
		const { data } = await CommentClient.getAllByPostId(
			postId,
			{
				Cursor: cursor,
				PageSize: 1,
				Direction: 'forward'
			},
			{ headers: authHeaders(accessToken) }
		).then((res) => res.data);

		if (!data?.items || data.items.length === 0) return { items: [], nextCursor: null };

		const userIds = data.items.map((comment) => comment.userId) as string[];
		const userMap = await getUsersByIds(accessToken, userIds);

		const commentsWithUsers = data.items.map((comment) => ({
			...comment,
			user: userMap.get(comment.userId) ?? null
		}));

		return {
			items: commentsWithUsers,
			nextCursor: data.pageInfo?.nextCursor ?? null
		};
	} catch (err) {
		console.error(err);
		return { items: [], nextCursor: null };
	}
}

export async function getRepliesByCommentId(
	accessToken: string,
	commentId: string,
	cursor?: string
) {
	try {
		const { data } = await CommentClient.getReplies(
			commentId,
			{
				Cursor: cursor,
				PageSize: 1,
				Direction: 'forward'
			},
			{ headers: authHeaders(accessToken) }
		).then((res) => res.data);

		if (!data?.items || data.items.length === 0) return { items: [], nextCursor: null };

		const userIds = data.items.map((reply) => reply.userId) as string[];
		const userMap = await getUsersByIds(accessToken, userIds);

		const repliesWithUsers = data.items.map((reply) => ({
			...reply,
			user: userMap.get(reply.userId) ?? null
		}));

		return {
			items: repliesWithUsers,
			nextCursor: data.pageInfo?.nextCursor ?? null
		};
	} catch (err) {
		console.error(err);
		return { items: [], nextCursor: null };
	}
}
