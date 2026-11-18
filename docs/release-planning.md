# Release planning

What is planned for each upcoming release: which Jira tickets, which database tags, and where each release is in the promotion path. The branching and release process itself is in `CONTRIBUTING.md`.

Update this page when a ticket is added to or moved out of a release, a database tag is committed, or a release moves to the next stage. Once a release is merged back to `develop`, move it to [Released](#released) and keep only its summary row.

Refer to work by branch name, pull request number, SVN revision or Helm revision, not by git commit hash. A branch's commit hashes change whenever it is rebased, so a recorded hash soon points at the wrong commit.

## Promotion path

Every release is validated in this order. A release moves on only after the checks for its current stage pass.

1. **localdev**: the docker-compose stack, pointed at the DEV databases.
2. **DEV**: `gitops/charts/deploy.ps1 dev`, against the DEV databases.
3. **TEST**: `deploy.ps1 test`, against the TEST databases.
4. **PROD**: point the citizen portal route (`citizen-web-for-justice-proxy`) at `maintenance-site`, run `deploy.ps1 prod`, and watch the rollout under Workloads > Deployments. Validate on PROD, point the route back at `traffic-court-online-citizen-web`, merge the release branch back to `develop`, then change the GitHub release from pre-release to latest (see `CONTRIBUTING.md`).

Database tags go to an environment's databases before (or with) the app release that needs them. localdev and DEV share the DEV databases, so a tag on DEV affects both.

## Summary

| Release | Status | Target | Tickets | Database tags |
|---|---|---|---|---|
| [2.31](#231) | Not started | — | TCVP-3598, TCVP-3589, TCVP-3602 | None expected |
| [2.32](#232) | In development | — | TCVP-3538, TCVP-3549, TCVP-3600, maybe TCVP-3480 | TCO 26.6, OCCAM 26.6 |

Status values: Not started, In development, In release branch, Validated on DEV, Validated on TEST, PROD booked, In PROD, Merged to `develop`.

---

## 2.31

- **Status:** Not started.
- **Theme:** Hearings functionality.
- **Branch:** `release/2.31`, to be created from `develop`. 2.30 was merged back on 2026-11-18 (#2341), so it can be created now.
- **Ticket branches:** local only, branched from `develop`.
- **Database tags:** none expected. Confirm for each ticket.

| Ticket | Branch | Summary | Database change? |
|---|---|---|---|
| TCVP-3598 | `task/TCVP-3598` | | Not expected |
| TCVP-3589 | `task/TCVP-3589` | | Not expected |
| TCVP-3602 | `task/TCVP-3602` | | Not expected |

**Things to know**

- The DEV databases already have TCO and OCCAM 26.6 (2.32 work), but 2.31 testing on DEV runs the 2.31 app. See [Database tags ahead of the app](#database-tags-ahead-of-the-app).
- If a 2.31 ticket turns out to need a database change, do not build its tag from SVN `trunk` as usual. Trunk already contains the 26.6 changes, so a tag copied from trunk would take 2.32 work to TEST and PROD early. Build that tag from the package versions deployed on PROD instead, then merge the change into trunk.

---

## 2.32

- **Status:** In development.
- **Theme:** Database-related updates.
- **Branch:** `release/2.32`, to be created from `develop` after 2.31 is merged back.

| Ticket | Summary | Status | Database change? |
|---|---|---|---|
| TCVP-3538 | Adjournment document flag on update requests; document types as an enum through the stack | Merged into 2.30 as #2328, then reverted. To be re-landed | Commit notes say the ORDS endpoint now returns the dispute GUID. Confirm whether a tag is needed |
| TCVP-3549 | | | |
| TCVP-3600 | VTC lock lasted about 8 h instead of 1 h. `VTC_ASSIGNED_DTM` now uses `SYSDATE`. JJ unassign timeout is configurable (`UNASSIGN_JJDISPUTES_TIMEOUT_MINUTES`). Inbox "Assigned" tooltip shows the expected unlock time | Validated on localdev. Committed on the local branch `task/TCVP-3600` (branched from `develop`), not yet pushed | Yes: TCO 26.6, OCCAM 26.6 |
| TCVP-3480 (maybe) | Dispute source type (`DISPUTE_SOURCE_TYPE_CD`) through the stack, and its child tickets | App code on `bcgov/fix/tcvp-3480`. Inclusion not decided | Yes: TCO 26.6, OCCAM 26.6 |

**Database tags**

| Repo | Tag | Revision | Contents |
|---|---|---|---|
| TCO-API-TCO | `tags\26.x\26.6\TCO\` | 185335 | TCVP-3480: `pDisputeSourceTypeCd` on dispute save. TCVP-3600: `VTC_ASSIGNED_DTM = SYSDATE` |
| TCO-API-OCCAM | `tags\26.x\26.6\OCCAM\` | 185334 | TCVP-3480: `pDisputeSourceTypeCd` on dispute save. TCVP-3600: `user_assigned_dtm` in the dispute and update-request list queries |

| Environment | TCO 26.6 | OCCAM 26.6 |
|---|---|---|
| DEV | Deployed and validated 2026-11-10 | Deployed and validated 2026-11-10 |
| TEST | No | No |
| PROD | No | No |

**Things to know**

- The 26.6 tags contain the TCVP-3480 database changes whether or not TCVP-3480's app code ships in 2.32. The new parameters default to `NULL`, so existing app code keeps working against them. The `DISPUTE_SOURCE_TYPE_CD` columns are added by the database team outside this repo; they exist on DEV.
- TCVP-3600's TCO tag and Staff.Service change must reach an environment in the same window. Either one alone shows the JJ dispute VTC-assigned time about 7 h off.
- Staff.Service now needs `UNASSIGN_DISPUTES_TIMEOUT_MINUTES` and `UNASSIGN_JJDISPUTES_TIMEOUT_MINUTES`. They are set once in Helm `global.env` and shared with oracle-data-api.
- After TCO 26.6 is deployed to an environment, clear VTC locks set under the old UTC rule once: `tco.tco_dispute.pDisputeUnassignVtc(SYSDATE, NULL)`.
- TCVP-3538 and TCVP-3600 both change `Staff.Service/Features/CourtFiles/Summaries/Handler.cs` and `Staff.Service/Features/Occam/DisputesWithUpdateRequests/Handler.cs`. Expect a small merge conflict for whichever lands second.

**Checklist**

- [x] TCO 26.6 and OCCAM 26.6 committed to SVN and noted in Jira
- [x] Database tags deployed to DEV databases
- [x] TCVP-3600 validated on localdev
- [ ] Decide whether TCVP-3480 is in this release
- [ ] Create `release/2.32`; merge ticket branches (squash)
- [ ] Deploy and validate on DEV
- [ ] Deploy 26.6 tags to TEST databases with the app; clear old VTC locks
- [ ] Validate on TEST
- [ ] Deploy 26.6 tags to PROD databases with the app; clear old VTC locks
- [ ] Validate on PROD; merge `release/2.32` into `develop`

---

## Cross-release notes

### Database tags ahead of the app

The DEV databases have TCO and OCCAM 26.6 from 2026-11-10, ahead of 2.32. Until 2.32 is deployed to DEV, the app on DEV (2.30, then 2.31) runs against them:

- Everything works. The 26.6 changes are backwards-compatible with 2.30 and 2.31 code.
- The VTC-assigned time on JJ disputes shows about 7 h early. The older Staff.Service still converts it from UTC.
- JJ dispute VTC locks clear after about 1 h, the older hard-coded timeout.
- A localdev stack running 2.32 code also runs its unassign jobs against the DEV databases. It clears locks for everyone on DEV at its own timeout.

TEST and PROD do not have 26.6, so 2.30 and 2.31 behave normally there.

### Open follow-ups

- **TCVP-3583 and TCVP-3586:** planned for 2.30, but no commits for either are in the release. Confirm in Jira whether they shipped earlier, need moving to a later release, or need no code.
- **Stale Redis node on PROD:** each oracle-data-api pod logs one or two `Unable to connect to [10.97.41.84:6379]` warnings at startup. That address is not one of the current `redis-0` to `redis-5` pods, so the Redis cluster most likely still lists an old node. Seen 2026-11-18 during the 2.30 deploy; not caused by 2.30.

---

## Template

Copy this when adding a release.

```markdown
## 2.NN

- **Status:** Not started.
- **Theme:**
- **Branch:** `release/2.NN`, to be created from `develop` after 2.MM is merged back.
- **Database tags:** none expected.

| Ticket | Summary | Status | Database change? |
|---|---|---|---|
| TCVP-NNNN | | | |

**Things to know**

-

**Checklist**

- [ ] Create `release/2.NN`; merge ticket branches (squash)
- [ ] Deploy and validate on DEV
- [ ] Deploy and validate on TEST
- [ ] Deploy and validate on PROD; merge `release/2.NN` into `develop`
```

## Released

| Release | PROD date | Notes |
|---|---|---|
| 2.30 | 2026-11-18 | Helm revision 57. Merged to `develop` in #2341. Planned: TCVP-3583, TCVP-3586, TCVP-3597. Also shipped: TCVP-3596, TCVP-3599, TCVP-3601, #2329, #2330 and the agent docs (`tcvp/3587`). TCVP-3538 was reverted out of 2.30 and moved to 2.32. No database tags. See [Open follow-ups](#open-follow-ups) |
