# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["BlogApp.Web/BlogApp.Web.csproj", "BlogApp.Web/"]
RUN dotnet restore "BlogApp.Web/BlogApp.Web.csproj"
COPY . .

WORKDIR /src/BlogApp.Web

RUN dotnet publish -c Release -o --no-restore /app/publish

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT [ "dotnet", "BlogApp.Web.dll" ]