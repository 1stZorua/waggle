import type { User } from '@waggle/api-client/user/model';

type UserModalMode = 'actions' | 'delete';

type UserModalState = {
	isOpen: boolean;
	mode: UserModalMode;
	profile: User | null;
	onSuccess?: () => void;
};

function createUserModal() {
	const state = $state<UserModalState>({
		isOpen: false,
		mode: 'actions',
		profile: null
	});

	function openActions(user: User, onSuccess?: () => void) {
		state.isOpen = true;
		state.mode = 'actions';
		state.profile = user;
		state.onSuccess = onSuccess;
	}

	function setMode(mode: UserModalMode) {
		state.mode = mode;
	}

	function close() {
		state.isOpen = false;
		setTimeout(() => {
			state.mode = 'actions';
			state.profile = null;
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
		get profile() {
			return state.profile;
		},
		openActions,
		setMode,
		close,
		onSuccess
	};
}

export const useUserModal = createUserModal();
