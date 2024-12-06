#!/bin/bash

dotnet nuget add source --name Artifactory https://artifactory.aws.wiley.com/artifactory/api/nuget/nuget
dotnet watch run --project=Authorization.API --urls=http://+:5001 --no-launch-profile