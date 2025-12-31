import type { Post } from '$lib/types/types';

type PostModalMode = 'actions' | 'create' | 'edit' | 'delete';

type PostModalState = {
	isOpen: boolean;
	mode: PostModalMode;
	post: Post | null;
	onSuccess?: () => void;
};

function createPostModal() {
	const state = $state<PostModalState>({
		isOpen: false,
		mode: 'actions',
		post: null,
		onSuccess: undefined
	});

	function openActions(post: Post, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'actions';
		state.post = post;
		state.onSuccess = onSuccess;
	}

	function openCreate(post?: Post, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'create';
		state.post = post ?? null;
		state.onSuccess = onSuccess;
	}

	function setMode(mode: PostModalMode) {
		state.mode = mode;
	}

	function close() {
		state.isOpen = false;
		setTimeout(() => {
			state.mode = 'actions';
			state.post = null;
			state.onSuccess = undefined;
		}, 300);
	}

	function triggerSuccess() {
		state.onSuccess?.();
	}

	return {
		get isOpen() {
			return state.isOpen;
		},
		set isOpen(value: boolean) {
			state.isOpen = value;
		},
		get mode() {
			return state.mode;
		},
		get post() {
			return state.post;
		},
		openActions,
		openCreate,
		setMode,
		close,
		triggerSuccess
	};
}

export const usePostModal = createPostModal();
