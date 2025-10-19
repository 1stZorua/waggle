import { json } from '@sveltejs/kit';

export function GET() {
	const uptimeSeconds = Math.floor(process.uptime());

	const health = {
		status: 'ok',
		uptime: uptimeSeconds,
		timestamp: new Date().toISOString()
	};

	return json(health);
}
