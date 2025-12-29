import type { PostMediaUrls } from '@waggle/api-client/post/model';
import type { User as UserModel } from '@waggle/api-client/user/model';

export interface Auth {
	accessToken: string;
	refreshToken: string;
}

export interface User {
	id: string;
	username: string;
	email: string;
	name: string;
	roles: string[];
}

export interface Comment {
	id?: string;
	user?: User | null;
	content?: string;
	createdAt?: string;
	commentIds?: number[];
}

export interface Post {
	id?: string;
	userId?: string;
	caption?: string | null;
	thumbnailId: string;
	mediaIds?: string[] | null;
	mediaUrls?: PostMediaUrls;
	likeCount?: number;
	commentCount?: number;
	createdAt?: string;
	user?: UserModel | null;
}

export type FlashType = 'success' | 'info' | 'warning' | 'error';
