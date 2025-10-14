import { writeFileSync } from 'fs';
import fetch from 'node-fetch';
import path from 'path';

const services = [
  { name: 'auth', port: 8081 },
  { name: 'user', port: 8080 }
];

const outputDir = path.resolve('./openapi');

for (const { name, port } of services) {
  const url = `http://localhost:${port}/swagger/v1/swagger.json`;
  fetch(url)
    .then((res) => res.json())
    .then((spec) => {
      const filePath = path.join(outputDir, `${name}.json`);
      writeFileSync(filePath, JSON.stringify(spec, null, 2));
      console.log(`Saved spec for ${name} at ${filePath}`);
    })
    .catch((err) => {
      console.error(`Failed to fetch spec for ${name}:`, err);
    });
}
