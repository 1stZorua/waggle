import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export function randomUser() {
  const id = uuidv4();
  return {
    username: `user_${id}`,
    email: `user_${id}@example.com`,
    firstName: `First_${id}`,
    lastName: `Last_${id}`,
    password: 'Test1234!'
  };
}

export function randomSleep(min = 1, max = 3) {
  return min + Math.random() * (max - min);
}
