import { LikeClient } from '@waggle/api-client/like';
import type { InteractionType } from '@waggle/api-client/like/model';
import { authHeaders } from './auth';

export async function like(accessToken: string, targetId: string, targetType: InteractionType) {
	return await LikeClient.create(
		{ targetId, targetType },
		{ headers: authHeaders(accessToken) }
	).then((res) => res.data);
}

export async function unlike(accessToken: string, id: string) {
	return await LikeClient.delete(id, { headers: authHeaders(accessToken) }).then((res) => res.data);
}

export async function hasLiked(accessToken: string, userId: string, targetId: string) {
	return await LikeClient.hasLiked(userId, targetId, {
		headers: authHeaders(accessToken)
	}).then((res) => res.data);
}
