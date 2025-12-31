export function timeAgo(dateString: string): string {
	const then = new Date(dateString).getTime();
	if (isNaN(then)) return '';

	let value = Math.floor((Date.now() - then) / 1000);
	value = Math.max(0, value);

	const units: [number, string][] = [
		[60, 's'],
		[60, 'm'],
		[24, 'h'],
		[30, 'd'],
		[12, 'mo'],
		[Infinity, 'y']
	];

	for (const [limit, unit] of units) {
		if (value < limit) return `${value}${unit}`;
		value = Math.floor(value / limit);
	}

	return `${value}y`;
}

export function formatTimestamp(dateString: string): string {
	const date = new Date(dateString);
	if (isNaN(date.getTime())) return '';

	const now = new Date();
	const diffMs = now.getTime() - date.getTime();
	const diffHours = diffMs / (1000 * 60 * 60);

	const isToday = date.toDateString() === now.toDateString();
	const yesterday = new Date(now);
	yesterday.setDate(yesterday.getDate() - 1);
	const isYesterday = date.toDateString() === yesterday.toDateString();

	if (isToday || isYesterday || diffHours < 168) {
		const hours = date.getHours();
		const minutes = date.getMinutes();
		const ampm = hours >= 12 ? 'PM' : 'AM';
		const displayHours = hours % 12 || 12;
		const displayMinutes = minutes.toString().padStart(2, '0');
		return `${displayHours}:${displayMinutes} ${ampm}`;
	}

	const month = date.toLocaleDateString('en-US', { month: 'short' });
	const day = date.getDate();

	if (date.getFullYear() !== now.getFullYear()) {
		return `${month} ${day}, ${date.getFullYear()}`;
	}

	return `${month} ${day}`;
}

export function formatNumber(count: number | undefined): string {
	if (count === undefined) return '0';

	const units = [
		{ value: 1e9, symbol: 'B' },
		{ value: 1e6, symbol: 'M' },
		{ value: 1e3, symbol: 'K' }
	];

	for (const { value, symbol } of units) {
		if (count >= value) {
			const formatted = count / value;
			return formatted >= 10
				? Math.floor(formatted) + symbol
				: formatted.toFixed(1).replace(/\.0$/, '') + symbol;
		}
	}

	return count.toString();
}

export function formatText(
	count: number | undefined,
	displayValue: string,
	singular: string,
	plural?: string
): string {
	const value = count ?? 0;
	const word = value === 1 ? singular : (plural ?? `${singular}s`);

	return `${displayValue} ${word}`;
}
