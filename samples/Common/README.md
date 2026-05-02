# Sample Common

Shared fixtures for the v2.0 Source Generator sample matrix live here.

Sprint 0 keeps this directory deliberately small. As Sprint 1+ fills in real sample
code, put cross-sample contracts, entities, and smoke-test helpers here only when two
or more samples genuinely need the same type.

Current shared skeleton:

- `SampleEntity` is a dependency-light record that samples can reuse when they need
  a neutral DTO or fixture value without pulling in a module-specific domain model.

Guidelines:

- Do not add framework registrations or generator-specific setup here.
- Do not make individual samples depend on common code just to reduce a few lines of
  local test data.
- Keep shared types small, deterministic, and safe for NativeAOT sample builds.