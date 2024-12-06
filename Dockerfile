FROM mcr.microsoft.com/dotnet/aspnet:6.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ADD --chmod=644 https://truststore.pki.rds.amazonaws.com/global/global-bundle.pem /cert/global-bundle.pem

WORKDIR /cert/

RUN cat global-bundle.pem|awk 'split_after==1{n++;split_after=0} /-----END CERTIFICATE-----/ {split_after=1} {print > "cert" n ""}' ;\
    for CERT in /cert/cert*; do mv $CERT /usr/local/share/ca-certificates/aws-rds-ca-$(basename $CERT).crt; done ;\
    update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim AS build
WORKDIR /src

ARG ART_USER
ARG ART_PASS
ARG ART_URL

RUN dotnet nuget add source --name crossknowledge/phoenix $ART_URL --username $ART_USER --password $ART_PASS --store-password-in-clear-text
RUN mkdir Authorization.API
COPY ./Authorization.API/Authorization.API.csproj ./Authorization.API/
RUN mkdir Authorization.Domain
COPY ./Authorization.Domain/Authorization.Domain.csproj ./Authorization.Domain/
RUN mkdir Authorization.Infrastructure
COPY ./Authorization.Infrastructure/Authorization.Infrastructure.csproj ./Authorization.Infrastructure/
RUN mkdir Authorization.Infrastructure.Interface
COPY ./Authorization.Infrastructure.Interface/Authorization.Infrastructure.Interface.csproj ./Authorization.Infrastructure.Interface/
RUN mkdir Authorization.Services
COPY ./Authorization.Services/Authorization.Services.csproj ./Authorization.Services/
COPY . .
RUN dotnet restore "Authorization.API/Authorization.API.csproj"
WORKDIR "/src/Authorization.API"
RUN dotnet build "Authorization.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authorization.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Authorization.API.dll"]
