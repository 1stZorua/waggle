export interface Auth {
	accessToken: string;
	refreshToken: string;
}

export type FlashType = 'success' | 'info' | 'warning' | 'error';
