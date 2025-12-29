import type { InteractionType } from '@waggle/api-client/like/model';
import { authHeaders } from './auth';
import { FavoriteClient } from '@waggle/api-client/favorite';

export async function favorite(accessToken: string, targetId: string, targetType: InteractionType) {
	return await FavoriteClient.create(
		{ targetId, targetType },
		{ headers: authHeaders(accessToken) }
	).then((res) => res.data);
}

export async function unfavorite(accessToken: string, id: string) {
	return await FavoriteClient.delete(id, { headers: authHeaders(accessToken) }).then(
		(res) => res.data
	);
}

export async function hasFavorited(accessToken: string, userId: string, targetId: string) {
	return await FavoriteClient.hasFavorited(userId, targetId, {
		headers: authHeaders(accessToken)
	}).then((res) => res.data);
}
