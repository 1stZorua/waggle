import type { User } from '@waggle/api-client/user/model';

type AvatarModalMode = 'actions' | 'edit';

type AvatarModalState = {
	isOpen: boolean;
	mode: AvatarModalMode;
	avatar: User | null;
	onSuccess?: () => void;
};

function createAvatarModal() {
	const state = $state<AvatarModalState>({
		isOpen: false,
		mode: 'actions',
		avatar: null
	});

	function openActions(avatar: User, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'actions';
		state.avatar = avatar;
		state.onSuccess = onSuccess;
	}

	function setMode(mode: AvatarModalMode) {
		state.mode = mode;
	}

	function close() {
		state.isOpen = false;
		setTimeout(() => {
			state.mode = 'actions';
			state.avatar = null;
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
		get avatar() {
			return state.avatar;
		},
		openActions,
		setMode,
		close,
		onSuccess
	};
}

export const useAvatarModal = createAvatarModal();
