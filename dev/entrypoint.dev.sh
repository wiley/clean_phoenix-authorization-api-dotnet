#!/bin/sh

dotnet nuget add source --name Artifactory https://artifactory.aws.wiley.com/artifactory/api/nuget/nuget
dotnet watch --project /app/Authorization.API run --verbosity normal --urls=http://+:80 --no-launch-profile
