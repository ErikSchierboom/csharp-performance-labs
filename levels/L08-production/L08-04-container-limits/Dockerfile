# Lab L08-04: builds the mystery service and runs it in a Linux container so the memory limit
# (docker/podman --memory, a cgroup) works the same on Windows, macOS and Linux.
# Build context is this lab folder; run.ps1 does the build for you.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/container-limits.csproj src/Program.cs ./
# Same settings the repo's Directory.Build.props applies to every lab (workstation, non-concurrent GC).
RUN printf '%s\n' '<Project><PropertyGroup>' \
    '<TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings>' \
    '<InvariantGlobalization>true</InvariantGlobalization>' \
    '<ConcurrentGarbageCollection>false</ConcurrentGarbageCollection><ServerGarbageCollection>false</ServerGarbageCollection>' \
    '</PropertyGroup></Project>' > Directory.Build.props
RUN dotnet publish container-limits.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "service.dll", "--seconds", "8", "--live-mb", "150"]
