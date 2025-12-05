export function randomUser() {
  const id = Math.floor(Math.random() * 1_000_000).toString(36);
  const timestamp = Date.now().toString(36);
  const vuPart = __VU.toString(36);

  const uniqueId = `${vuPart}${timestamp}${id}`.slice(0, 18);
  const username = `k6${uniqueId}`;

  return {
    username,
    email: `${username}@example.com`,
    firstName: `First`,
    lastName: `Last`,
    password: 'Test1234!',
    confirmPassword: 'Test1234!'
  };
}

export function randomSleep(min = 1, max = 3) {
  return min + Math.random() * (max - min);
}
