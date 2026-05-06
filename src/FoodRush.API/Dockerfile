FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/FoodRush.API/FoodRush.API.csproj", "src/FoodRush.API/"]
COPY ["src/FoodRush.Application/FoodRush.Application.csproj", "src/FoodRush.Application/"]
COPY ["src/FoodRush.Infrastructure/FoodRush.Infrastructure.csproj", "src/FoodRush.Infrastructure/"]
COPY ["src/FoodRush.Domain/FoodRush.Domain.csproj", "src/FoodRush.Domain/"]

RUN dotnet restore "src/FoodRush.API/FoodRush.API.csproj"

COPY . .

RUN dotnet publish "src/FoodRush.API/FoodRush.API.csproj" \
    -c Release \
    -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FoodRush.API.dll"]