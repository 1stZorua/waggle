import type { Comment } from '$lib/types/types';

type CommentModalMode = 'actions' | 'create' | 'edit' | 'delete';

type CommentModalState = {
	isOpen: boolean;
	mode: CommentModalMode;
	comment: Comment | null;
	onSuccess?: () => void;
};

function createCommentModal() {
	const state = $state<CommentModalState>({
		isOpen: false,
		mode: 'actions',
		comment: null
	});

	function openActions(comment: Comment, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'actions';
		state.comment = comment;
		state.onSuccess = onSuccess;
	}

	function openCreate(comment: Comment, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'create';
		state.comment = comment;
		state.onSuccess = onSuccess;
	}

	function setMode(mode: CommentModalMode) {
		state.mode = mode;
	}

	function close() {
		state.isOpen = false;
		setTimeout(() => {
			state.mode = 'actions';
			state.comment = null;
		}, 300);
	}

	function onSuccess() {
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
		get comment() {
			return state.comment;
		},
		openActions,
		openCreate,
		setMode,
		close,
		onSuccess
	};
}

export const useCommentModal = createCommentModal();
