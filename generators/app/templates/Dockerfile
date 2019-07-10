FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300 AS build-env
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

# publish
WORKDIR /app/src/StarterKit

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.5 AS runtime
WORKDIR /app
COPY --from=build-env /app/src/StarterKit/out .

RUN mkdir -p /log
#ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

CMD ["dotnet", "StarterKit.dll"]
