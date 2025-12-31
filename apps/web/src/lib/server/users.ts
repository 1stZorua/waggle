import { UserClient } from '@waggle/api-client/user';
import { authHeaders } from './auth';

export async function getUsersByIds(accessToken: string, userIds: string[]) {
	const uniqueUserIds = [...new Set(userIds.filter(Boolean))];

	if (uniqueUserIds.length === 0) {
		return new Map();
	}

	const usersResponse = await UserClient.getByIds(
		{ ids: uniqueUserIds as string[] },
		{ headers: authHeaders(accessToken) }
	);

	return new Map((usersResponse.data.data || []).map((u) => [u.id, u]));
}

export async function getUserById(accessToken: string, userId: string) {
	return await UserClient.getById(userId, {
		headers: authHeaders(accessToken)
	}).then((res) => res.data.data);
}
