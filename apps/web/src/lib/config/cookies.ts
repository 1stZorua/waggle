export const COOKIE_OPTIONS = {
	path: '/',
	httpOnly: true,
	secure: true,
	sameSite: 'none' as const
} as const;

export const MAX_AGE = 60 * 60 * 24 * 30;
