export const COOKIE_OPTIONS = {
	path: '/',
	httpOnly: true,
	secure: true,
	sameSite: 'lax' as const
} as const;

export const MAX_AGE = 60 * 60 * 24 * 30;
