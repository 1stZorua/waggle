import { check } from 'k6';

export function parseResponse(res) {
  try {
    return res.body ? JSON.parse(res.body) : null;
  } catch (e) {
    console.error(`Failed to parse response: ${e.message}`);
    return null;
  }
}

export function checkSuccessResponse(res, expectedStatus = 200) {
  const body = parseResponse(res);

  return check(res, {
    [`status is ${expectedStatus}`]: (r) => r.status === expectedStatus,
    'response is valid JSON': () => body !== null,
    'response has success status': () => body && body.status === 'success',
    'response contains data': () => body && body.data !== null
  });
}

export function checkErrorResponse(res, expectedStatus = 400) {
  const body = parseResponse(res);

  return check(res, {
    [`status is ${expectedStatus}`]: (r) => r.status === expectedStatus,
    'response is valid JSON': () => body !== null,
    'response has fail status': () => body && body.status === 'fail',
    'response has error message': () => body && body.message !== undefined,
    'response has error code': () => body && body.code !== undefined
  });
}

// Check user registration response
export function checkRegisterResponse(res, user) {
  const body = parseResponse(res);

  const basicChecks = checkSuccessResponse(res, 201);

  const dataChecks = check(res, {
    'data contains user id': () => body && body.data && body.data.id !== undefined,
    'data contains correct username': () =>
      body && body.data && body.data.username === user.username,
    'data contains correct email': () => body && body.data && body.data.email === user.email,
    'data contains correct firstName': () =>
      body && body.data && body.data.firstName === user.firstName,
    'data contains correct lastName': () =>
      body && body.data && body.data.lastName === user.lastName,
    'password not returned': () => body && body.data && body.data.password === undefined
  });

  return basicChecks && dataChecks;
}

// Check login response
export function checkLoginResponse(res) {
  const body = parseResponse(res);

  const basicChecks = checkSuccessResponse(res, 200);

  const authChecks = check(res, {
    'data contains access token': () => body && body.data && body.data.accessToken !== undefined,
    'data contains refresh token': () => body && body.data && body.data.refreshToken !== undefined,
    'data contains user info': () => body && body.data && body.data.user !== undefined
  });

  return basicChecks && authChecks;
}

// Generic check with custom validations
export function checkResponse(res, expectedStatus, customChecks = {}) {
  const body = parseResponse(res);

  const baseChecks = {
    [`status is ${expectedStatus}`]: (r) => r.status === expectedStatus,
    'response is valid JSON': () => body !== null
  };

  // Merge base checks with custom checks
  const allChecks = { ...baseChecks, ...customChecks };

  return check(res, allChecks);
}
