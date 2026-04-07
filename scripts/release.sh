#!/usr/bin/env bash
set -euo pipefail

# Usage: ./scripts/release.sh 0.2.0
# This tags and pushes, which triggers the GitHub Actions release workflow.

VERSION="${1:?Usage: $0 <version> (e.g. 0.2.0)}"

# Validate semver-ish format
if [[ ! "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9.]+)?$ ]]; then
  echo "Error: Version must be in semver format (e.g. 1.2.3 or 1.2.3-preview.1)"
  exit 1
fi

TAG="v${VERSION}"

# Update version in csproj
CSPROJ="src/StravaMcp.Server/StravaMcp.Server.csproj"
sed -i "s|<Version>.*</Version>|<Version>${VERSION}</Version>|" "$CSPROJ"

git add "$CSPROJ"
git commit -m "Bump version to ${VERSION}"
git tag -a "$TAG" -m "Release ${TAG}"
git push origin HEAD
git push origin "$TAG"

echo ""
echo "Tagged ${TAG} and pushed. GitHub Actions will:"
echo "  1. Build the project"
echo "  2. Pack the NuGet package (version ${VERSION})"
echo "  3. Push to nuget.org"
echo "  4. Create a GitHub Release"
