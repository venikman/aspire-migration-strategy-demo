# Aspire Migration Cost Calculator

> **Interactive calculator** to estimate costs and ROI for migrating from Aspire 9.5 to Aspire 13

---

## 📊 Quick Calculator

Fill in your values below to calculate potential savings:

### Step 1: Current Production Traffic

```
Daily Requests:        ____________  (e.g., 10,000)
Number of Services:    ____________  (e.g., 5)
Average Spans/Request: ____________  (default: 5-10)
```

**Calculation**:
```
Daily Spans = Daily Requests × Services × Avg Spans/Request

Example:
10,000 × 5 × 5 = 250,000 spans/day
```

---

### Step 2: Monthly Telemetry Costs

#### Current Costs (Aspire 9.5 - 100% Sampling)

```
Daily Spans:    ____________ (from Step 1)
Monthly Spans:  ____________ (Daily × 30)
```

**APM Provider Pricing** (choose yours):

| Provider | Cost per Million Spans | Notes |
|----------|------------------------|-------|
| **Application Insights** | ~$20 | Standard pricing tier |
| **Datadog APM** | ~$31 | Pro plan |
| **New Relic** | ~$25 | Standard data ingest |
| **Elastic APM** | ~$16 | Cloud hosted |
| **Jaeger (self-hosted)** | ~$5 | Infrastructure costs only |

```
Your APM Provider:     ____________
Cost per Million:      $____________

CURRENT MONTHLY COST (Aspire 9.5):
= (Monthly Spans ÷ 1,000,000) × Cost per Million
= $____________
```

#### Future Costs (Aspire 13 - 10% Sampling)

```
Aspire 13 reduces production sampling to 10%

Monthly Spans (10%): ____________ (Monthly Spans × 0.1)

FUTURE MONTHLY COST (Aspire 13):
= (Monthly Spans ÷ 1,000,000) × Cost per Million × 0.1
= $____________
```

**Monthly Savings**:
```
SAVINGS = Current Cost - Future Cost
        = $____________
```

---

### Step 3: Migration Cost Estimate

#### Direct Migration Costs

```
Migration Tasks                          Days    Daily Rate    Cost
────────────────────────────────────────────────────────────────────
.NET SDK upgrade (9 → 10)                 0.5    $________    $____
Update all .csproj files                  0.5    $________    $____
Update AppHost configuration              1.0    $________    $____
Replace AddNpmApp with AddViteApp         0.5    $________    $____
Add .WaitFor() dependencies               0.5    $________    $____
Update resilience configuration           1.0    $________    $____
Add ASPIRE_ALLOW_UNSECURED_TRANSPORT      0.5    $________    $____
Testing (unit, integration, E2E)          2.0    $________    $____
CI/CD pipeline updates                    1.0    $________    $____
Staging deployment + validation           0.5    $________    $____
Production deployment                     0.5    $________    $____
────────────────────────────────────────────────────────────────────
TOTAL MIGRATION DAYS:                     8.5 days

TOTAL MIGRATION COST:
= 8.5 × Daily Rate
= $____________
```

**Typical Daily Rates**:
- Junior Developer: $400-600/day
- Mid-Level Developer: $600-800/day
- Senior Developer: $800-1200/day
- Consultant: $1200-2000/day

---

### Step 4: ROI Calculation

```
Monthly Savings:       $____________ (from Step 2)
Migration Cost:        $____________ (from Step 3)

BREAK-EVEN TIME:
= Migration Cost ÷ Monthly Savings
= ____________ months

ANNUAL SAVINGS:
= Monthly Savings × 12
= $____________
```

---

## 📈 Real-World Examples

### Example 1: Small Application

```
Traffic Profile:
  Daily Requests:        1,000
  Services:              3
  Spans/Request:         5

Calculation:
  Daily Spans:           15,000
  Monthly Spans:         450,000

Costs (Application Insights at $20/M):
  Aspire 9.5 (100%):     $9/month
  Aspire 13 (10%):       $0.90/month
  Monthly Savings:       $8.10

Migration:
  Effort:                8.5 days
  Cost (@$800/day):      $6,800

ROI:
  Break-even:            840 months ❌
  Recommendation:        STAY ON 9.5
```

**Decision**: Cost savings too small to justify migration effort.

---

### Example 2: Medium Application

```
Traffic Profile:
  Daily Requests:        50,000
  Services:              5
  Spans/Request:         8

Calculation:
  Daily Spans:           2,000,000
  Monthly Spans:         60,000,000

Costs (Datadog at $31/M):
  Aspire 9.5 (100%):     $1,860/month
  Aspire 13 (10%):       $186/month
  Monthly Savings:       $1,674

Migration:
  Effort:                8.5 days
  Cost (@$800/day):      $6,800

ROI:
  Break-even:            4.1 months
  Annual Savings:        $20,088

  Recommendation:        MIGRATE ✅
```

**Decision**: ROI in 4 months, $20k annual savings.

---

### Example 3: Large Enterprise Application

```
Traffic Profile:
  Daily Requests:        500,000
  Services:              15
  Spans/Request:         10

Calculation:
  Daily Spans:           75,000,000
  Monthly Spans:         2,250,000,000

Costs (Application Insights at $20/M):
  Aspire 9.5 (100%):     $45,000/month
  Aspire 13 (10%):       $4,500/month
  Monthly Savings:       $40,500

Migration:
  Effort:                8.5 days
  Cost (@$1200/day):     $10,200

ROI:
  Break-even:            0.25 months (7.5 days!)
  Annual Savings:        $486,000

  Recommendation:        MIGRATE NOW! 🚀
```

**Decision**: ROI in 1 week, nearly half a million in annual savings!

---

## 💡 Hidden Costs & Benefits

### Additional Costs (Aspire 13)

```
Item                              One-Time    Recurring
────────────────────────────────────────────────────────
Team training (1 hour)            $400        -
Documentation updates             $200        -
Monitoring setup (new features)   $300        -
────────────────────────────────────────────────────────
TOTAL ADDITIONAL COSTS:           $900        -
```

### Additional Benefits (Aspire 13)

```
Benefit                           Annual Value    How Calculated
──────────────────────────────────────────────────────────────────
Faster developer productivity      $__________    (reduced wait times)
  - 40% faster dashboard
  - 2x faster container startup

Reduced incident response time     $__________    (fewer production issues)
  - Built-in resilience
  - Better error handling

Avoided manual Polly config        $__________    (saved development time)
  - 50 lines → 1 line
  - ~4 hours saved per service

Better debugging experience        $__________    (reduced MTTR)
  - .WaitFor() eliminates race conditions
  - Clearer error messages
──────────────────────────────────────────────────────────────────────
TOTAL ADDITIONAL BENEFITS:         $__________
```

---

## 🎯 Decision Thresholds

Use these guidelines to make your decision:

```
┌──────────────────────────────────────────────────────────┐
│             MIGRATION DECISION MATRIX                    │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  Monthly Savings < $100:                                 │
│  → ❌ DON'T MIGRATE (savings too small)                 │
│  → Wait for .NET 10 LTS (Nov 2025)                      │
│                                                          │
│  Monthly Savings $100-$500:                              │
│  → 🤔 CONSIDER (break-even 6-12 months)                 │
│  → Factor in additional benefits                        │
│                                                          │
│  Monthly Savings $500-$1,000:                            │
│  → ✅ MIGRATE (break-even 3-6 months)                   │
│  → Strong business case                                 │
│                                                          │
│  Monthly Savings > $1,000:                               │
│  → 🚀 MIGRATE NOW (break-even < 3 months)               │
│  → Urgent business imperative                           │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## 📋 Cost Tracking Spreadsheet

**Copy this template to Excel/Google Sheets**:

```csv
Metric,Aspire 9.5,Aspire 13,Savings
Daily Requests,,N/A,
Number of Services,,N/A,
Spans per Request,,N/A,
Daily Spans,=A2*A3*A4,=A2*A3*A4*0.1,=B5-C5
Monthly Spans,=B5*30,=C5*30,=B6-C6
Cost per Million Spans,,,
Monthly Telemetry Cost,=(B6/1000000)*B7,=(C6/1000000)*B7,=B8-C8
Annual Telemetry Cost,=B8*12,=C8*12,=B9-C9
,,,
Migration Effort (days),,8.5,
Daily Developer Rate,,$800,
Migration Cost,,=C12*C13,
,,,
Break-even (months),,=C14/B8,
Annual Savings,,=B9-C9,
ROI (%),,=(C17-C14)/C14*100,
```

---

## 🔧 Using This Calculator

### Option 1: Manual Calculation

1. Fill in your values in Step 1-3
2. Calculate using the formulas provided
3. Compare against decision thresholds

### Option 2: Excel Spreadsheet

1. Copy the CSV template above
2. Paste into Excel/Google Sheets
3. Enter your values in columns A/B
4. Formulas auto-calculate savings and ROI

### Option 3: Interactive Tool (Future)

```bash
# Coming soon: CLI calculator
npx aspire-cost-calculator \
  --requests 10000 \
  --services 5 \
  --spans 5 \
  --apm-cost 20
```

---

## 📊 Sensitivity Analysis

How do different factors affect ROI?

### Impact of Traffic Volume

```
Requests/Day    Monthly Cost (9.5)    Savings    Break-even
────────────────────────────────────────────────────────────
1,000           $9                    $8         840 months ❌
10,000          $90                   $81        84 months ❌
50,000          $450                  $405       17 months 🤔
100,000         $900                  $810       8 months ✅
500,000         $4,500                $4,050     2 months 🚀
1,000,000       $9,000                $8,100     1 month 🚀
```

### Impact of APM Provider

```
Provider               Cost/M Spans    Monthly (100k req/day)    Savings
────────────────────────────────────────────────────────────────────────
Elastic APM            $16             $720                      $648
Application Insights   $20             $900                      $810
New Relic              $25             $1,125                    $1,013
Datadog                $31             $1,395                    $1,256
```

### Impact of Developer Rate

```
Daily Rate    Migration Cost    Break-even (@$810 savings/month)
───────────────────────────────────────────────────────────────────
$400          $3,400            4.2 months
$600          $5,100            6.3 months
$800          $6,800            8.4 months
$1,200        $10,200           12.6 months
```

---

## ⚠️ Important Assumptions

This calculator assumes:

1. **Sampling Rates**:
   - Aspire 9.5: 100% (no environment-based sampling)
   - Aspire 13: 10% in production, 100% in dev/staging

2. **Migration Effort**: 8.5 days (industry average)
   - Your actual effort may vary based on:
     - Complexity of existing Polly configurations
     - Number of services
     - Team familiarity with Aspire

3. **Telemetry Overhead**: Only considers span ingestion costs
   - Does NOT include: storage, retention, query costs
   - These typically add 20-30% to total APM costs

4. **No Service Interruptions**: Assumes zero downtime migration
   - Any production incidents would add cost

---

## 🤝 Next Steps

Based on your calculations:

### If Migrating:

1. [ ] Review [ADR](../ADR_ASPIRE_MIGRATION_9_TO_13.md) for detailed plan
2. [ ] Check [Feature Comparison](../docs/feature-implementation-comparison.md)
3. [ ] Use [Decision Tree](../docs/decision-tree.md) to confirm
4. [ ] Schedule migration sprint
5. [ ] Update CI/CD pipelines (see `.azure-pipelines/`)

### If Staying on 9.5:

1. [ ] Document decision using this calculator
2. [ ] Set reminder for .NET 10 LTS (Nov 2025)
3. [ ] Consider implementing Aspire 13 patterns manually:
   - Environment-based sampling
   - Standard resilience handlers
4. [ ] Monitor telemetry costs monthly

---

**Last Updated**: 2025-12-17
**Calculator Version**: 1.0
**Applies to**: .NET Aspire 9.5 → 13 migration decisions
