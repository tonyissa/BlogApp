# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY "BlogApp.Web.csproj" ./
RUN dotnet restore "BlogApp.Web.csproj"
COPY . .

RUN dotnet publish -c Release -o --no-restore /app/publish

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT [ "dotnet", "BlogApp.Web.dll" ]