# Set the base image as the .NET 5.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./
RUN dotnet publish ./VersionGenerator/VersionGenerator.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Randhall <kazimierz.luska@gmail.com>"
LABEL repository="https://github.com/Randhall/VersionGenerator"
LABEL homepage="https://github.com/Randhall/VersionGenerator"

# Label as GitHub action
LABEL com.github.actions.name="Version Generator"
LABEL com.github.actions.description="A GitHub Action that will generate a version string for publishing artefacts based on the branch and build time"
LABEL com.github.actions.icon="sliders"
LABEL com.github.actions.color="purple"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/VersionGenerator.dll" ]
