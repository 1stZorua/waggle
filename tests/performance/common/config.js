export const CONFIG = {
  services: {
    auth: __ENV.AUTH_SERVICE_BASE_URL || 'http://localhost:9081',
    user: __ENV.USER_SERVICE_BASE_URL || 'http://localhost:9083'
  },

  scenarios: {
    smoke: {
      vus: 1,
      duration: '1m'
    },
    load: {
      vus: 10,
      duration: '1m'
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
