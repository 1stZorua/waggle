import http from 'k6/http';
import { sleep } from 'k6';
import { CONFIG, getScenario } from '../common/config.js';
import { randomUser, randomSleep } from '../common/helpers.js';
import { checkRegisterResponse } from '../common/checks.js';

const scenario = getScenario(__ENV.SCENARIO || 'load');

export let options = {
  ...scenario,
  thresholds: CONFIG.defaults.thresholds
};

const AUTH_BASE_URL = CONFIG.services.auth;

export default function () {
  const user = randomUser();

  const res = http.post(`${AUTH_BASE_URL}/api/auth/register`, JSON.stringify(user), {
    headers: { 'Content-Type': 'application/json' },
    tags: { name: 'RegisterUser' }
  });

  const success = checkRegisterResponse(res, user);

  if (!success || (res.status !== 200 && res.status !== 201)) {
    sleep(randomSleep(0.5, 2));
    return;
  }

  let userId;
  try {
    const response = JSON.parse(res.body);
    userId = response.data?.id;

    if (!userId) {
      console.warn('Registration succeeded but no user ID in response');
      sleep(randomSleep(0.5, 2));
      return;
    }
  } catch (e) {
    console.error('Failed to parse registration response:', e);
    sleep(randomSleep(0.5, 2));
    return;
  }

  sleep(0.1);

  const delRes = http.del(`${AUTH_BASE_URL}/api/auth/${userId}`, null, {
    headers: { 'Content-Type': 'application/json' },
    tags: { name: 'DeleteUser' },
    timeout: '10s'
  });

  if (delRes.status !== 200 && delRes.status !== 204) {
    console.warn(`Failed to delete user ${userId}: ${delRes.status}`);
  }

  sleep(randomSleep(0.5, 2));
}

export function setup() {
  const res = http.get(`${AUTH_BASE_URL}/health`, { timeout: '10s' });
  if (res.status !== 200) {
    throw new Error(`API health check failed: ${res.status}`);
  }
  console.log('✓ API is healthy and ready for testing');
}

export function teardown() {
  console.log('✓ Test completed');
}
