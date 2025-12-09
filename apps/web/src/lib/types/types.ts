export interface Auth {
	accessToken: string;
	refreshToken: string;
}

export interface User {
	id: string;
	username: string;
	email: string;
	name: string;
}

export type FlashType = 'success' | 'info' | 'warning' | 'error';
