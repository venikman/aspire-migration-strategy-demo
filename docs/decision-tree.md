# .NET Aspire Migration Strategy - Decision Tree

> **Interactive guide** to help you choose between migrating now (.NET 10 + Aspire 13) or staying on .NET 9 + Aspire 9.5

---

## 🎯 Quick Decision Matrix

```
┌─────────────────────────────────────────────────────────────────────┐
│                     START HERE                                      │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              ▼
              ┌───────────────────────────────┐
              │ Are you building a NEW        │
              │ project from scratch?         │
              └───────┬───────────────────────┘
                      │
         ┌────────────┴────────────┐
         │                         │
        YES                       NO
         │                         │
         ▼                         ▼
    ┌─────────────┐      ┌─────────────────────┐
    │ Use .NET 10 │      │ Do you have an      │
    │ + Aspire 13 │      │ EXISTING project?   │
    │             │      └──────┬──────────────┘
    │ ✅ DONE     │             │
    └─────────────┘    ┌────────┴────────┐
                       │                  │
                      YES                NO
                       │                  │
                       ▼                  ▼
            ┌──────────────────┐    [Continue below]
            │ What .NET version│
            │ are you on?      │
            └────┬─────────────┘
                 │
    ┌────────────┼────────────┐
    │            │            │
  .NET 8      .NET 9     .NET 6/7
    │            │            │
    ▼            ▼            ▼
[Continue]  [Continue]   [Continue]
  below       below         below
```

---

## 📊 Detailed Decision Path

### Path 1: New Project (Greenfield)

```
┌─────────────────────────────────────────────────────────────┐
│ NEW PROJECT                                                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✅ RECOMMENDATION: Use .NET 10 + Aspire 13                │
│                                                             │
│  REASONS:                                                   │
│  • No migration cost - start with latest                   │
│  • Built-in resilience patterns (one line vs 50)           │
│  • Environment-based sampling (90% cost savings)            │
│  • First-class Vite/React support                          │
│  • .WaitFor() service dependencies                         │
│                                                             │
│  RISKS:                                                     │
│  ⚠️  .NET 10 is preview (but production-ready)             │
│  ⚠️  Rapid version changes (expect updates)                │
│                                                             │
│  ACTION ITEMS:                                              │
│  1. Install .NET 10 SDK                                    │
│  2. Run: dotnet new aspire-starter                         │
│  3. Follow this repo's patterns                            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

### Path 2: Existing Project on .NET 9

```
┌─────────────────────────────────────────────────────────────┐
│ EXISTING PROJECT - .NET 9                                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Question: How long until .NET 10 LTS?                     │
│  └─ Answer: .NET 10 LTS expected Nov 2025                  │
│                                                             │
│  🤔 DECISION POINT:                                         │
│  ┌─────────────────────────────────────┐                   │
│  │ Can you wait 10-11 months?         │                   │
│  └───────┬─────────────────────────────┘                   │
│          │                                                  │
│    ┌─────┴─────┐                                           │
│   YES          NO                                           │
│    │           │                                            │
│    ▼           ▼                                            │
│  STRATEGY A  STRATEGY B                                     │
│  (Stay 9.5)  (Migrate now)                                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘

STRATEGY A: Stay on Aspire 9.5 (Conservative)
──────────────────────────────────────────────
✅ Pros:
  • Zero migration effort now
  • Stable .NET 9 LTS
  • Wait for .NET 10 LTS (Nov 2025)
  • One big migration later (.NET 9→10 + Aspire 9.5→13+)

❌ Cons:
  • Miss out on cost savings ($900/month)
  • Manual Polly configuration
  • Slower dashboard (40% slower)
  • No .WaitFor() dependencies
  • Higher telemetry costs (100% sampling)

⏱️  Timeline:
  • Now: Stay on .NET 9 + Aspire 9.5
  • Nov 2025: Migrate to .NET 10 LTS + Aspire 13+
  • Total migrations: 1 (in 10-11 months)

💰 Cost:
  • Migration effort: 0 days (now), 5-7 days (Nov 2025)
  • Telemetry cost: $900/month × 11 = $9,900 extra

─────────────────────────────────────────────────────────────

STRATEGY B: Migrate to Aspire 13 Now (Aggressive)
──────────────────────────────────────────────────
✅ Pros:
  • Immediate cost savings ($900/month)
  • Better developer experience (40% faster dashboard)
  • Built-in resilience (one line of code)
  • .WaitFor() service dependencies
  • Only one migration (.NET 9 supported by Aspire preview)

❌ Cons:
  • Migration effort now (5-7 days)
  • .NET 10 is preview (but production-ready)
  • Team learning curve

⏱️  Timeline:
  • Now: Migrate to .NET 10 preview + Aspire 13
  • Nov 2025: Upgrade to .NET 10 LTS (minor upgrade)
  • Total migrations: 1 (now) + 1 minor upgrade (Nov 2025)

💰 Cost:
  • Migration effort: 5-7 days (now), 1 day (Nov 2025)
  • Telemetry savings: $900/month × 11 = $9,900 saved
  • ROI: Pays for itself in Week 1

─────────────────────────────────────────────────────────────

🎯 RECOMMENDATION for .NET 9 projects:

IF:
  • Production telemetry costs > $500/month → STRATEGY B
  • High traffic application → STRATEGY B
  • Need resilience patterns → STRATEGY B
  • Risk-averse team → STRATEGY A
  • Can wait for .NET 10 LTS → STRATEGY A
```

---

### Path 3: Existing Project on .NET 8

```
┌─────────────────────────────────────────────────────────────┐
│ EXISTING PROJECT - .NET 8                                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ⚠️  RECOMMENDATION: Use phased approach                    │
│                                                             │
│  REASON:                                                    │
│  • .NET 8 → 10 is a BIG jump (skip .NET 9)                │
│  • Minimize risk with incremental upgrades                 │
│                                                             │
│  PHASED MIGRATION PATH:                                     │
│  ┌───────────────────────────────────────────┐             │
│  │ Phase 1: .NET 8 → .NET 9 (Now)           │             │
│  │   • Test for breaking changes             │             │
│  │   • Stay on Aspire 9.5                    │             │
│  │   • Duration: 2-3 weeks                   │             │
│  └───────────────────────────────────────────┘             │
│                     ▼                                       │
│  ┌───────────────────────────────────────────┐             │
│  │ Phase 2: .NET 9 → .NET 10 (3-6 months)   │             │
│  │   • Migrate to Aspire 13                  │             │
│  │   • Gain cost savings                     │             │
│  │   • Duration: 1-2 weeks                   │             │
│  └───────────────────────────────────────────┘             │
│                                                             │
│  ALTERNATIVE (Aggressive):                                  │
│  • Jump directly to .NET 10 + Aspire 13                    │
│  • Higher risk (test thoroughly)                           │
│  • Duration: 3-4 weeks                                     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

### Path 4: Existing Project on .NET 6/7 (End of Life)

```
┌─────────────────────────────────────────────────────────────┐
│ EXISTING PROJECT - .NET 6 or .NET 7                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🚨 URGENT: Both .NET 6 and .NET 7 are End of Life         │
│                                                             │
│  .NET 6: EOL November 2024                                 │
│  .NET 7: EOL May 2024                                      │
│                                                             │
│  ✅ RECOMMENDATION: Migrate to .NET 10 + Aspire 13         │
│                                                             │
│  REASON:                                                    │
│  • Already need a major migration                          │
│  • Might as well jump to latest                            │
│  • Security vulnerabilities in EOL versions                │
│                                                             │
│  MIGRATION PATH:                                            │
│  ┌───────────────────────────────────────────┐             │
│  │ Step 1: .NET 6/7 → .NET 9 (Stable)       │             │
│  │   • Test application thoroughly           │             │
│  │   • Fix breaking changes                  │             │
│  │   • Stay on existing architecture         │             │
│  │   • Duration: 3-4 weeks                   │             │
│  └───────────────────────────────────────────┘             │
│                     ▼                                       │
│  ┌───────────────────────────────────────────┐             │
│  │ Step 2: Add Aspire 9.5                   │             │
│  │   • Introduce observability               │             │
│  │   • Test in production                    │             │
│  │   • Duration: 2-3 weeks                   │             │
│  └───────────────────────────────────────────┘             │
│                     ▼                                       │
│  ┌───────────────────────────────────────────┐             │
│  │ Step 3: .NET 9 + Aspire 9.5 → 10 + 13    │             │
│  │   • Follow this repo's migration guide    │             │
│  │   • Duration: 1-2 weeks                   │             │
│  └───────────────────────────────────────────┘             │
│                                                             │
│  TOTAL TIMELINE: 6-9 weeks                                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚦 Risk Tolerance Assessment

Choose your risk profile:

```
┌─────────────────────────────────────────────────────────────┐
│                   RISK PROFILE                              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🟢 LOW RISK (Conservative)                                 │
│  ───────────────────────────                                │
│  Characteristics:                                           │
│  • Enterprise production environment                        │
│  • Strict change control processes                         │
│  • Long certification cycles                               │
│  • Can't afford downtime                                   │
│                                                             │
│  → STRATEGY: Stay on LTS versions                          │
│  → Wait for .NET 10 LTS (Nov 2025)                         │
│  → Use Aspire 9.5 with .NET 9                              │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🟡 MEDIUM RISK (Balanced)                                  │
│  ──────────────────────────                                 │
│  Characteristics:                                           │
│  • Business-critical but flexible                          │
│  • Good CI/CD and testing                                  │
│  • Can roll back if needed                                 │
│  • Cost-conscious                                          │
│                                                             │
│  → STRATEGY: Phased migration                              │
│  → Upgrade to .NET 10 preview now                          │
│  → Migrate to Aspire 13 for cost savings                   │
│  → Upgrade to .NET 10 LTS in Nov 2025                      │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🔴 HIGH RISK (Aggressive)                                  │
│  ──────────────────────                                     │
│  Characteristics:                                           │
│  • Startup or internal tooling                             │
│  • Fast iteration cycles                                   │
│  • Want latest features                                    │
│  • Developer productivity > stability                      │
│                                                             │
│  → STRATEGY: Immediate migration                           │
│  → Use .NET 10 + Aspire 13 now                             │
│  → Accept rapid version changes                            │
│  → Benefit from latest features                            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 💰 Cost-Based Decision

Use this if telemetry costs are a primary concern:

```
┌─────────────────────────────────────────────────────────────┐
│            MONTHLY TELEMETRY COST CALCULATOR                │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Step 1: Calculate your current costs                      │
│  ───────────────────────────────────                        │
│  Requests/day: ___________                                 │
│  Services:     ___________                                 │
│  Spans/request: __________ (avg 5-10)                      │
│                                                             │
│  Daily spans = Requests × Services × Spans/request         │
│  Monthly spans = Daily spans × 30                          │
│                                                             │
│  Cost (APM) = Monthly spans × $0.02 per 1000 spans        │
│               (typical Application Insights pricing)       │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Step 2: Calculate Aspire 13 savings                       │
│  ────────────────────────────────                           │
│  Aspire 9.5: 100% sampling                                 │
│  Aspire 13:   10% sampling (production)                    │
│                                                             │
│  Savings = Current cost × 0.90  (90% reduction)            │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Step 3: Decision threshold                                │
│  ──────────────────────────                                 │
│  IF Monthly savings > $300                                 │
│  THEN Migrate now (ROI < 2 weeks)                          │
│                                                             │
│  IF Monthly savings > $1000                                │
│  THEN Migrate ASAP (ROI < 1 week)                          │
│                                                             │
│  ELSE Stay on 9.5 until .NET 10 LTS                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘

Example Calculation:
────────────────────
10,000 requests/day × 5 services × 5 spans = 250,000 spans/day
250,000 × 30 = 7,500,000 spans/month

Aspire 9.5 cost: 7,500,000 ÷ 1000 × $0.02 = $1,500/month
Aspire 13 cost:    750,000 ÷ 1000 × $0.02 = $150/month
Savings: $1,350/month

Migration effort: 5 days × $800/day = $4,000
ROI: 4,000 ÷ 1,350 = 2.96 months ✅ Migrate now!
```

---

## 🎯 Final Decision Flowchart

```
                         START
                           │
                           ▼
              ┌────────────────────────┐
              │ Monthly telemetry      │
              │ cost > $500?           │
              └─────┬──────────────────┘
                    │
        ┌───────────┴───────────┐
       YES                      NO
        │                        │
        ▼                        ▼
  Migrate to           ┌─────────────────┐
  Aspire 13 NOW        │ Risk tolerance? │
  (Cost savings)       └────┬────────────┘
                            │
                   ┌────────┼────────┐
                  LOW    MEDIUM    HIGH
                   │        │        │
                   ▼        ▼        ▼
                Stay on  Phased   Migrate
                9.5      Migration  NOW
                until    (2-step)
                .NET 10
                LTS
```

---

## 📋 Action Items Checklist

Based on your decision, use this checklist:

### ✅ If Migrating to Aspire 13 NOW

- [ ] Read [ADR_ASPIRE_MIGRATION_9_TO_13.md](../ADR_ASPIRE_MIGRATION_9_TO_13.md)
- [ ] Install .NET 10 SDK on all dev machines
- [ ] Run migration scripts (when available)
- [ ] Update CI/CD pipelines (.NET SDK version)
- [ ] Test in staging environment
- [ ] Monitor telemetry costs for 1 week
- [ ] Train team on new Aspire 13 features

### ✅ If Staying on Aspire 9.5 (for now)

- [ ] Document decision (use this decision tree as rationale)
- [ ] Set calendar reminder for .NET 10 LTS (Nov 2025)
- [ ] Bookmark this repo for migration reference
- [ ] Consider manual implementation of Aspire 13 patterns:
  - [ ] Environment-based sampling
  - [ ] Standard resilience handlers
  - [ ] Service dependency management
- [ ] Monitor .NET 10 LTS release notes

---

## 🤔 Still Unsure?

**Ask yourself these questions**:

1. **Can my team afford 5-7 days of migration work now?**
   - Yes → Consider migrating
   - No → Stay on 9.5

2. **Are we spending > $500/month on telemetry?**
   - Yes → Migrate (immediate ROI)
   - No → Stay on 9.5

3. **Is our production environment stable and testable?**
   - Yes → Lower risk to migrate
   - No → Stay on 9.5 until stable

4. **Do we have good CI/CD and rollback procedures?**
   - Yes → Migrate with confidence
   - No → Stay on 9.5 until infrastructure improves

5. **Is our team comfortable with preview .NET versions?**
   - Yes → Migrate
   - No → Wait for .NET 10 LTS (Nov 2025)

---

## 📞 Need Help?

- Review the [ADR](../ADR_ASPIRE_MIGRATION_9_TO_13.md) for detailed analysis
- Check the [Version Comparison](../ASPIRE_VERSION_COMPARISON.md) for visual guides
- See [Feature Comparison](./feature-implementation-comparison.md) for code examples
- Use the [Cost Calculator](../tools/aspire-cost-calculator.xlsx) for financial analysis

---

**Last Updated**: 2025-12-17
**Applies to**: .NET Aspire 9.5 → 13 migration decisions
