type FollowState<T extends { id: string }> = {
	obj: T | null;
	pending: boolean | null;
	count?: number;
};

export function useToggleFollow<T extends { id: string }>(
	userId: string | undefined,
	initialCount?: number
) {
	const state = $state<FollowState<T>>({
		obj: null,
		pending: null,
		count: initialCount
	});

	async function init() {
		if (!userId) return;

		try {
			const res = await fetch(`/api/follows/check/${userId}`);
			if (!res.ok) return;

			const json = await res.json();
			state.obj = json?.data ?? null;
		} catch (error) {
			console.error('Failed to check follow', error);
		}
	}

	async function toggle() {
		if (!userId) return;

		const wasActive = state.obj !== null;
		const prevObj = state.obj;
		const prevCount = state.count;

		state.pending = !wasActive;
		if (state.count !== undefined) state.count += wasActive ? -1 : 1;

		try {
			if (wasActive && state.obj) {
				await fetch(`/api/follows`, {
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ id: state.obj.id })
				});
				state.obj = null;
			} else {
				const res = await fetch(`/api/follows`, {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ followingId: userId })
				});
				const json = await res.json();
				state.obj = json?.data ?? null;
			}
		} catch (error) {
			console.error('Failed to toggle follow', error);
			state.obj = prevObj;
			state.count = prevCount;
		} finally {
			state.pending = null;
		}
	}

	return {
		get active() {
			return state.pending ?? state.obj !== null;
		},
		get count() {
			return state.count;
		},
		get obj() {
			return state.obj;
		},
		init,
		toggle
	};
}
