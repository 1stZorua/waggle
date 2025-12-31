type ToggleState<T extends { id: string }> = {
	active: boolean;
	obj: T | null;
	count?: number;
};

export function useToggleReaction<T extends { id: string }>(
	targetId: string | undefined,
	type: 'likes' | 'favorites',
	targetType?: 'post' | 'comment',
	initialCount?: number
) {
	const state = $state<ToggleState<T>>({
		active: false,
		obj: null,
		count: initialCount
	});

	async function init() {
		if (!targetId) return;

		try {
			const res = await fetch(`/api/${type}/check/${targetId}`);
			const text = await res.text();
			const data = text ? JSON.parse(text) : null;

			state.active = !!data;
			state.obj = data ?? null;
		} catch (e) {
			console.error(`Failed to check ${type}`, e);
		}
	}

	async function toggle() {
		if (!targetId) return;

		const prev = { ...state };
		state.active = !state.active;
		if (state.count !== undefined) state.count += state.active ? 1 : -1;

		try {
			if (state.active) {
				const res = await fetch(`/api/${type}`, {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ targetId, targetType })
				});

				state.obj = (await res.json())?.data ?? null;
			} else {
				await fetch(`/api/${type}`, {
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify({ id: state.obj?.id })
				});
				state.obj = null;
			}
		} catch (e) {
			console.error(`Failed to toggle ${type}`, e);
			Object.assign(state, prev);
		}
	}

	return {
		get active() {
			return state.active;
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
