#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KiBoards/KiBoards.csproj", "KiBoards/"]
RUN dotnet restore "KiBoards/KiBoards.csproj"
COPY . .
WORKDIR "/src/KiBoards"
RUN dotnet build "KiBoards.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KiBoards.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KiBoards.dll"]