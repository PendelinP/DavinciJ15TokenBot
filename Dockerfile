FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-dotnet
WORKDIR /src
COPY . .
RUN dotnet restore "DavinciJ15TokenBot/DavinciJ15TokenBot.csproj"
WORKDIR "/src/DavinciJ15TokenBot"
RUN dotnet build "DavinciJ15TokenBot.csproj" -c DockerRelease -o /app

FROM build-dotnet AS publish
RUN dotnet publish "DavinciJ15TokenBot.csproj" -c DockerRelease -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DavinciJ15TokenBot.dll"]