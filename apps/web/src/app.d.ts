import type { Auth, FlashType, Post, User } from '$lib/types/types';

// See https://kit.svelte.dev/docs/types#app
// for information about these interfaces
declare global {
	namespace App {
		// interface Error {}
		// interface Locals {}
		interface PageData {
			flash?: { type: FlashType; message: string };
			user?: User;
		}

		interface PageState {
			post: Post;
		}
		// interface Platform {}
		interface Locals {
			auth: Auth;
			user: User;
		}
	}
}

export {};
