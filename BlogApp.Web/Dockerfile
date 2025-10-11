# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY "BlogApp.Web.csproj" ./
RUN dotnet restore "BlogApp.Web.csproj"
COPY . .

RUN dotnet publish -c Release -o /app/publish "BlogApp.Web.csproj"

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT [ "dotnet", "BlogApp.Web.dll" ]