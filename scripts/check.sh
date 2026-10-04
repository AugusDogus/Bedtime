#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

dotnet test tests/Bedtime.Tests/Bedtime.Tests.csproj -c Release \
  ${MANAGED_DIR:+-p:ManagedDir="$MANAGED_DIR"} \
  ${BEPINEX_DIR:+-p:BepInExDir="$BEPINEX_DIR"}
bun run scripts/validate-package.ts
