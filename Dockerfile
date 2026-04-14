# Stage 1 — restore (cached NuGet layer, invalidated only by .csproj changes)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY src/Api/Api.csproj                         src/Api/
COPY src/Application/Application.csproj         src/Application/
COPY src/Domain/Domain.csproj                   src/Domain/
COPY src/Infrastructure/Infrastructure.csproj   src/Infrastructure/
RUN dotnet restore src/Api/Api.csproj

# Stage 2 — build and publish
FROM restore AS publish
WORKDIR /src
COPY src/ src/
RUN dotnet publish src/Api/Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Stage 3 — runtime image (no SDK, minimal footprint)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
