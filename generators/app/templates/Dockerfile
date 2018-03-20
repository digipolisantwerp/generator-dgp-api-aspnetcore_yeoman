FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app
 
# Copy csproj and restore as distinct layers
COPY src/StarterKit/*.csproj ./
COPY nuget.config ./
RUN dotnet restore --configfile ./nuget.config
 
# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/src/StarterKit/out .

RUN mkdir -p /log
#ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

CMD ["dotnet", "StarterKit.dll"]
