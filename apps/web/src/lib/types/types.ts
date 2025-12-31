import type { User as UserModel } from '@waggle/api-client/user/model';
import type { Comment as CommentModel } from '@waggle/api-client/comment/model';
import type { Post as PostModel } from '@waggle/api-client/post/model';

export interface Auth {
	accessToken: string;
	refreshToken: string;
}

export interface AuthUser {
	id: string;
	username: string;
	email: string;
	name: string;
	roles: string[];
}

export type Comment = CommentModel & {
	user?: UserModel | null;
};

export type Post = PostModel & {
	user?: UserModel | null;
	comments?: Comment[] | null;
	commentsNextCursor?: string | null;
};

export type FlashType = 'success' | 'info' | 'warning' | 'error';
