export function timeAgo(dateString: string): string {
	const then = new Date(dateString).getTime();
	if (isNaN(then)) return '';

	let value = Math.floor((Date.now() - then) / 1000);

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
