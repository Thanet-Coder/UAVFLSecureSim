# UAVFLSecureSim

**A C# research prototype for investigating model poisoning and trust-aware aggregation in UAV-based federated learning.**

## Project Overview

UAVFLSecureSim simulates a swarm of unmanned aerial vehicles (UAVs) collaboratively training a machine-learning model using federated learning (FL).

The application investigates how compromised UAVs can affect the global model through model-poisoning attacks and whether trust-aware aggregation can reduce their influence.

The research compares standard Federated Averaging (FedAvg) with a multi-evidence TrustAware aggregation mechanism.

The application is implemented in C# using .NET 8.

## Simulation Environment

The prototype uses a fixed 15 × 10 wildfire temperature map.

Five simulated UAVs survey separate geographical regions, collecting 30 temperature observations each. Their local datasets are non-independent and identically distributed (non-IID), reflecting geographical differences in the simulated environment.

Each UAV trains a local logistic-regression model. The local model parameters are submitted to a federated coordinator, which produces an updated global model over ten communication rounds.

Compromised UAVs can submit poisoned model updates.

This is a software simulation; no physical UAVs or real-world wireless networks are involved.

## Federated Aggregation Methods

### Standard FedAvg

Standard Federated Averaging combines submitted model parameters using the number of local training samples associated with each update.

It does not incorporate trust scores when calculating aggregation weights.

### TrustAware Aggregation

TrustAware aggregation evaluates four types of evidence:

| Trust component | Weight |
|---|---:|
| Behaviour consistency | 25% |
| Validation performance | 40% |
| Temporal consistency | 25% |
| Communication reliability | 10% |

Each submitted update receives an overall trust score.

Aggregation weights are calculated using:

`Aggregation Weight = Sample Count × Overall Trust`

These weights are then normalised.

TrustAware aggregation reduces the influence of lower-trust updates rather than automatically excluding them.

## Model-Poisoning Investigation

The application simulates compromised UAVs submitting deliberately manipulated model parameters.

Seven predefined benchmark scenarios examine combinations of aggregation method, compromised UAV proportion and communication reliability.

| Scenario | Aggregation | Compromised UAVs | Communication reliability |
|---|---|---:|---:|
| A | FedAvg | 0% | 100% |
| B | TrustAware | 0% | 100% |
| C | FedAvg | 20% | 100% |
| D | TrustAware | 20% | 100% |
| E | FedAvg | 40% | 100% |
| F | TrustAware | 40% | 100% |
| G | TrustAware | 20% | 80% |

The benchmark configuration uses a poisoning attack strength of 2.0.

## Running the Application

### Requirements

- .NET 8 SDK
- Visual Studio 2022 with .NET development support, or a compatible .NET development environment

### Visual Studio

1. Open the UAVFLSecureSim solution or project.
2. Build the solution.
3. Run the application.
4. Choose one of the two main-menu options.

### Command Line

From the directory containing `UAVFLSecureSim.csproj`, run:

```bash
dotnet run
```

The application presents:

```text
1. Interactive Simulation
2. Benchmark Suite
```

**Interactive Simulation** allows users to inspect the simulated UAV survey, federated training process, trust evidence and final global-model evaluation.

**Benchmark Suite** executes the seven predefined research scenarios and exports results to CSV files.

For reproducibility, the benchmark suite is the recommended option.

## Benchmark Outputs

The benchmark suite produces three CSV files in the `results` directory:

| File | Contents |
|---|---|
| `benchmark_results.csv` | Accuracy, precision, recall, F1 score, log loss, communication counts and execution time |
| `benchmark_timing.csv` | Execution-time statistics from repeated benchmark runs |
| `benchmark_detection.csv` | Malicious-update detection metrics, including true positives, false positives, true negatives and false negatives |

Model-quality scenarios are executed using seeds 42, 101, 202, 303 and 404.

The timing benchmark uses warm-up runs and repeated measurements.

Detection results are evaluated using an overall trust threshold of 0.40. This threshold is used for classification and reporting; it does not act as a hard exclusion rule within TrustAware aggregation.

## Selected Research Results

Under the tested configuration with 40% compromised UAVs:

| Metric | FedAvg | TrustAware |
|---|---:|---:|
| Accuracy | 28.6% | 95.2% |
| Precision | 35.3% | 90.9% |
| Recall | 60.0% | 100.0% |
| F1 score | 44.4% | 95.2% |
| Log loss | 6.3052 | 0.2651 |

TrustAware aggregation substantially outperformed FedAvg under this particular poisoning scenario.

These findings demonstrate the behaviour of the tested implementations and configuration. They do not establish general robustness against arbitrary attacks.

## Trust Mechanism Investigation

An additional investigation compared the original weighted trust mechanism with an experimental validation-conditioned temporal trust mechanism.

The experimental mechanism improved malicious-update detection recall in one scenario but produced substantially more false positives when all UAVs were honest.

The original weighted trust mechanism therefore remains the default.

Further details are recorded in `Milestone10_Findings.md`.

## Research Limitations

- The UAVs, wildfire environment and communication conditions are simulated.
- The map is small and fixed, with five UAVs in the predefined benchmark scenarios.
- The local data distributions are non-IID but limited in complexity.
- The experiments use a specific model-poisoning implementation and attack strength.
- Model-quality results are identical across the five tested seeds in scenarios A–F, limiting the interpretation of seed-based repetition.
- Detection decisions and poisoning mitigation are separate mechanisms.
- Detection observations across federated rounds are not independent samples.
- The prototype does not establish real-world UAV security, privacy guarantees or deployment suitability.
- The experiments do not measure physical communication performance or energy consumption.

## Future Research

Possible extensions include more realistic UAV environments, alternative poisoning strategies, improved validation-based trust mechanisms, larger swarms, diverse data distributions and evaluation under real communication constraints.

The prototype is