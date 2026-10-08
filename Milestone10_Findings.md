# Milestone 10 — Trust Mechanism Investigation

## Research Objective

Investigate whether modifications to the trust-combination mechanism can improve malicious-update detection without unfairly penalising honest UAVs.

Examine the relationship between malicious-update detection and the ability of trust-aware aggregation to mitigate model poisoning.

## Mechanisms Examined

**1. Original weighted trust**

Combines four trust components:

- Behaviour: 25%
- Validation: 40%
- Temporal consistency: 25%
- Communication reliability: 10%

**2. Validation-conditioned temporal trust**

An experimental mechanism that multiplies temporal trust by validation trust before combining the four components.

The original mechanism remains the default implementation.

## Experimental Detection Findings

All results below use detection threshold 0.40 and random seed 42.

### Scenario F — 40% Malicious, TrustAware

| Mechanism | TP | FP | TN | FN | Recall |
|---|---:|---:|---:|---:|---:|
| Original | 12 | 0 | 30 | 8 | 60% |
| Experimental | 18 | 0 | 30 | 2 | 90% |

The experimental mechanism improved detection recall in this scenario.

### Scenario B — All Honest, TrustAware

| Mechanism | FP | TN | False-positive rate |
|---|---:|---:|---:|
| Original | 2 | 48 | 4% |
| Experimental | 34 | 16 | 68% |

The experimental mechanism incorrectly classified a substantial proportion of honest updates as suspicious.

Consequently, the improved recall observed in Scenario F does not establish that the experimental mechanism is superior overall.

## Explanation of the Experimental Results

The experimental mechanism calculates:

Adjusted Temporal Trust = Temporal Trust × Validation Trust

When validation trust approaches zero, the temporal component contributes little or nothing to the overall trust score.

The original validation evaluator uses relative min-max normalisation across received updates.

This means an honest UAV can receive low validation trust simply because its submitted model performs worse than other submissions, even when it improves performance relative to the current global model.

This behaviour is particularly relevant when UAVs hold non-IID training data.

## Global-Relative Validation Investigation

A diagnostic evaluator was implemented to compare each submitted model's validation log loss with that of the pre-aggregation global model.

The diagnostic calculates:

Log-Loss Difference = Submitted Model Log Loss − Global Model Log Loss

Positive values indicate deterioration relative to the current global model. Negative values indicate improvement.

### Scenario B — Round 1

All five honest UAV submissions reduced validation log loss relative to the global model.

Nevertheless, two honest UAVs received zero relative validation trust.

This demonstrates that zero relative validation trust does not necessarily indicate a harmful update.

### Scenario F — Round 7

Both malicious UAV submissions increased validation log loss, while all three honest submissions reduced it.

However, malicious UAV-01 received an overall trust score of approximately 0.5178 and was therefore not classified as suspicious at threshold 0.40.

Its temporal trust was 1.0000, illustrating how consistent malicious update directions can receive high temporal-consistency scores.

These observations suggest that global-relative validation evidence deserves further investigation, but they do not establish a generally reliable detection method.

## Detection Versus Poisoning Mitigation

The TrustAware aggregator does not use the detection threshold to exclude UAVs.

Instead, each received update is weighted using:

Aggregation Weight = Sample Count × Overall Trust

The resulting weights are normalised before aggregating the submitted model parameters.

In the current five-UAV, 15 × 10 map configuration, each UAV receives 30 observations.

For Scenario F, Round 7, the two malicious UAVs received approximately 26% of the combined aggregation weight, compared with 40% under equal-sample FedAvg.

This demonstrates reduced relative aggregation influence, although the actual impact of an update also depends on its model parameters.

The distinction between detection and mitigation is important: a malicious UAV may escape threshold-based detection while still receiving less aggregation influence than honest UAVs.

## Model-Quality Findings

In the existing benchmark suite, Scenario E (40% malicious, FedAvg) achieved approximately 28.6% model accuracy and 6.3052 log loss.

Scenario F (40% malicious, TrustAware) achieved approximately 95.2% accuracy and 0.2651 log loss.

These results demonstrate substantially better model performance for TrustAware under the tested attack configuration.

They should not be interpreted as evidence of equivalent protection across other datasets, attack methods or real UAV deployments.

## Limitations

- Experiments use simulated UAVs and a small, fixed wildfire environment.
- Local datasets are non-IID but deterministic under the tested configuration.
- The five random seeds produce identical model-quality results in scenarios A–F, limiting their value as independent experimental variations.
- The poisoning investigation uses a particular attack mechanism and strength.
- The experimental trust comparison was exploratory and was not independently validated on unseen scenarios.
- Different trust mechanisms produce different global-model trajectories, complicating direct comparisons.
- Received updates from successive rounds are not independent experimental repetitions.
- Detection performance and global-model quality must be evaluated separately.
- Global-relative validation diagnostics have not yet been evaluated systematically across all rounds and scenarios.

## Conclusions

The original TrustAware mechanism demonstrated substantial resilience to model poisoning in the tested scenarios.

The experimental validation-conditioned temporal mechanism improved malicious-update detection recall in Scenario F but caused an unacceptable increase in false positives in the honest Scenario B.

The investigation also revealed limitations in relative validation scoring and highlighted the importance of distinguishing malicious-update detection from poisoning mitigation.

The original weighted trust mechanism is therefore retained as the default.

## Future Research

Potential future investigations include:

- Global-relative validation scoring across multiple rounds and attack conditions.
- Alternative approaches to balancing temporal consistency and validation evidence.
- More diverse non-IID data distributions.
- Additional poisoning strategies and attack strengths.
- Larger UAV swarms and more realistic communication conditions.
- Independent evaluation of detection accuracy, model robustness and computational overhead.

These investigations are outside the scope of Milestone 10.