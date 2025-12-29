import { goto } from '$app/navigation';

export async function refreshTokenClient() {
	try {
		const res = await fetch('/api/auth/refresh', {
			method: 'POST',
			credentials: 'include'
		});

		if (!res.ok) goto('/login');
	} catch (err) {
		console.error('Error refreshing token:', err);
		goto('/login');
	}
}

export function startTokenRefresh(intervalMs: number = 5 * 60 * 1000) {
	refreshTokenClient();

	const id = setInterval(() => {
		refreshTokenClient();
	}, intervalMs);

	return id;
}

export function stopTokenRefresh(id: ReturnType<typeof setInterval>) {
	clearInterval(id);
}
