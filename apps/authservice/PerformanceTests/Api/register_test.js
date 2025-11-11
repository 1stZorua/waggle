import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, randomUser } from './common.js'; // adjust path if needed

export let options = {
  vus: 10, // number of virtual users
  duration: '30s', // test duration
  thresholds: {
    http_req_duration: ['p(95)<500'] // 95% of requests should be below 500ms
  }
};

export default function () {
  const user = randomUser();

  const res = http.post(`${BASE_URL}/api/auth/register`, JSON.stringify(user), {
    headers: { 'Content-Type': 'application/json' }
  });

  // Add this to see what's going wrong
  console.log(`Status: ${res.status}`);
  console.log(`URL: ${BASE_URL}/api/auth/register`);
  console.log(`Body: ${res.body}`);
  console.log(`Error: ${res.error}`);

  check(res, {
    'status is 200': (r) => r.status === 201,
    'body is valid JSON': (r) => {
      try {
        JSON.parse(r.body);
        return true;
      } catch (e) {
        return false;
      }
    },
    'body contains username': (r) => r.body.includes(user.username)
  });

  sleep(1);
}
