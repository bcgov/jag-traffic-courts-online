# Front end source code

This directory contains the two Angular portals.

## Apps

| Name           | Users                     | Angular | Node (Docker build image) | Backing API (host port) | Doc |
| -------------- | ------------------------- | ------- | ------------------------- | ----------------------- | --- |
| citizen-portal | Public disputants         | 16      | 18.12                     | citizen-api (5080)      | [README](citizen-portal/README.md) |
| staff-portal   | VTC staff and JJs         | 21      | 24                        | staff-api (5090)        | [README](staff-portal/README.md) |

Both portals use Yarn (`yarn.lock`). The Dockerfiles run `yarn install`, so use Yarn rather than npm when changing dependencies. Use a Node version that matches the app's Docker build image. The two portals need different versions, so a version manager such as `nvm` helps.

## Run locally

Start the backing API with docker-compose from the repository root (see the [root README](../../README.md#run-docker-compose)). Then, in the portal directory:

```bash
yarn install
yarn start
```

`yarn start` proxies `/api` to the backing API using `proxy.conf.json`.

| Portal         | `yarn start` serves on   | docker-compose serves on |
| -------------- | ------------------------ | ------------------------ |
| citizen-portal | http://localhost:5200    | http://localhost:8080    |
| staff-portal   | http://localhost:4200    | http://localhost:8090    |

## Build, lint, and test

Run these in the portal directory:

| Task | Command |
| ---- | ------- |
| Lint | `yarn lint` |
| Unit tests (headless, single run) | `yarn coverage` |
| Development build | `yarn build-dev` |
| Production build | `yarn build-prod` |
| Regenerate API client from a running API's Swagger | `yarn api:regenerate` |

In citizen-portal, `yarn test` starts Karma in watch mode, so use `yarn coverage` for a single run. In staff-portal, `yarn test` already runs once, headless. CI lints and tests citizen-portal (`.github/workflows/main-code-climate-coverage-aggregation.yml`) and builds its Docker image on pull requests (`.github/workflows/pr-frontend.yml`).

## Troubleshooting

- ZScaler can interfere with opening the portals in a browser. Try disabling and re-enabling it.
- If a Docker build fails with "Call retries were exceeded" and exit code 127, increase Docker Desktop's memory limit.
