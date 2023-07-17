FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
 
# Copy csproj and restore as distinct layers
COPY src/StarterKit/*.csproj ./

COPY ./test/StarterKit.UnitTests/*.csproj ./test/StarterKit.UnitTests/

COPY nuget.config ./

RUN dotnet restore --configfile ./nuget.config
 
# Copy everything else
COPY . ./

# build and run tests
WORKDIR /app/src/StarterKit
RUN dotnet build

WORKDIR /app/test
RUN dotnet test StarterKit.UnitTests
RUN dotnet test StarterKit.IntegrationTests

# publish
WORKDIR /app/src/StarterKit

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/src/StarterKit/out .

# Inject version
ARG release
ENV RELEASE_VERSION=$release

RUN mkdir -p /log
#ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

# Set timezone
RUN ln -snf /usr/share/zoneinfo/Europe/Brussels /etc/localtime && echo Europe/Brussels > /etc/timezone

CMD ["dotnet", "StarterKit.dll"]
