# Contributing

All Contributions to this repository start with a Jira Item associated with the work request.

## Branching and release strategy

`develop` is the project's main branch. `main` is not used. It has not been updated since release 2.3 (September 2024), so do not branch from it or target it.

1. **Work branch.** Implement every change on a branch created from `develop`. Name it after the Jira item, for example `fix/TCVP-1234`.
1. **Release branch.** When preparing a release, create a version-numbered branch `release/#.##.##` from `develop`.
1. **Into the release.** Raise a pull request from each work branch that is pending for the release into the release branch. Complete it with **Squash merge**.
1. **Validation.** Deploy the release branch to DEV, then TEST, for validation passes.
1. **Production.** Once validated, deploy the release branch to PROD.
1. **Back to develop.** Once the PROD deployment is validated, raise a pull request from the release branch into `develop`. Complete it with a **Merge commit**, not a squash, so the release history is preserved.

```text
develop ──┬──────────────────────────────────────────────┬──▶
          │ branch                     merge commit (PR) │
          ├─▶ fix/TCVP-1234 ──┐                          │
          │                   │ squash (PR)              │
          └─▶ release/#.##.## ◀┘ ─▶ DEV ─▶ TEST ─▶ PROD ──┘
```

## Pull Request Process

1. Create a branch from `develop`. If you do not have write access, fork the repository to your personal GitHub space first (see [working with forks](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/working-with-forks)).
1. [Build](README.md#run-docker-compose) and run the application on your local environment. Build and test commands for each component are in [src/backend/README.md](src/backend/README.md#build-and-test) and [src/frontend/README.md](src/frontend/README.md#build-lint-and-test).
1. Commit your changes to your branch.
1. Update the README.md with details of changes to the interface, this includes new environment 
   variables, exposed ports, useful file locations and container parameters.
1. When your change is ready to be reviewed, open a PR into the target `release/#.##.##` branch using our [Pull Request Template](.github/pull_request_template.md) and optionally assign one or more reviewer. Also, in the comment of the Jira item, put a link to the Pull Request and move the item to `in review`
1. If some changes are required, you will be notified in the PR, address any change requested and push to the same branch.
1. When your change is meeting the requirement, the reviewer will squash merge it into the release branch, you can celebrate! It reaches `develop` when the release branch is merged back after PROD validation.

## AI agent context

`AGENTS.md` at the repository root routes AI coding agents to the documentation they need for a task. Agent-only resources, such as review personas and skills, live under `.agents/`. Shared documentation stays in this file, `README.md`, and `docs/`. See `docs/decisions.md` (001, 002).

When you move or rename a document that `AGENTS.md` references, update the route. Then run the validator from Git Bash or another POSIX shell:

```bash
.agents/skills/validate-context-routes/scripts/validate.sh
```

## Open a new bug in jira

- When a bug is detected in the application, a jira bug must be open
    the bug should be documented as followed:
        - Expected Behavior
        - Current Behavior
        - Steps to Reproduce
        - Environment
        - Description (stack traces, screenshot, error messages, splunk logs query)
        - Optionally you can suggest a possible solution
    - set the target release

## Open a new feature request in jira

- For feature enhancements, a jira task or story must be open
    - set the target release
