FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY src/KiBoards/bin/Release/net7.0/linux-x64/publish .

ENTRYPOINT ["dotnet", "KiBoards.dll"]