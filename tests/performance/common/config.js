export const CONFIG = {
  services: {
    auth: __ENV.AUTH_SERVICE_BASE_URL || 'https://authservice.waggle.local',
    user: __ENV.USER_SERVICE_BASE_URL || 'https://userservice.waggle.local'
  },

  admin: {
    username: __ENV.ADMIN_USERNAME || 'zorua',
    password: __ENV.ADMIN_PASSWORD || 'test'
  },

  scenarios: {
    smoke: {
      vus: 1,
      duration: '1m'
    },
    load: {
      stages: [
        { duration: '10s', target: 2 },
        { duration: '20s', target: 5 },
        { duration: '30s', target: 10 },
        { duration: '10s', target: 0 }
      ]
    },
    stress: {
      stages: [
        { duration: '2m', target: 10 },
        { duration: '5m', target: 50 },
        { duration: '2m', target: 100 },
        { duration: '5m', target: 100 },
        { duration: '2m', target: 0 }
      ]
    },
    spike: {
      stages: [
        { duration: '10s', target: 10 },
        { duration: '1m', target: 100 },
        { duration: '10s', target: 10 }
      ]
    }
  },

  defaults: {
    thresholds: {
      http_req_duration: ['p(95)<500', 'p(99)<1000'],
      http_req_failed: ['rate<0.01'],
      checks: ['rate>0.95']
    },
    setupTimeout: '60s',
    teardownTimeout: '60s'
  }
};

export function getScenario(scenarioName = 'load') {
  return CONFIG.scenarios[scenarioName] || CONFIG.scenarios.load;
}
