## Releasing a New Version

Releases are fully automated via GitHub Actions. To publish a new version:

```bash
./scripts/release.sh 0.2.0
```

This will:
1. Update the version in the `.csproj`
2. Commit, tag (`v0.2.0`), and push
3. GitHub Actions builds, packs, and pushes to **nuget.org**
4. A GitHub Release is created with release notes and the `.nupkg` attached

### One-time setup

Add your NuGet API key as a repository secret:
1. Go to **Settings > Secrets and variables > Actions**
2. Add `NUGET_API_KEY` with your [nuget.org API key](https://www.nuget.org/account/apikeys)

## License

MIT
