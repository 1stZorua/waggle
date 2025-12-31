import { authHeaders } from './auth';
import { FollowClient } from '@waggle/api-client/follow';

export async function follow(accessToken: string, followingId: string) {
	return await FollowClient.create({ followingId }, { headers: authHeaders(accessToken) }).then(
		(res) => res.data
	);
}

export async function unfollow(accessToken: string, id: string) {
	return await FollowClient.delete(id, { headers: authHeaders(accessToken) }).then(
		(res) => res.data
	);
}

export async function isFollowing(accessToken: string, followerId: string, followingId: string) {
	return await FollowClient.isFollowing(followerId, followingId, {
		headers: authHeaders(accessToken)
	}).then((res) => res.data);
}
