FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Auction.sln", "./"]
COPY ["AuctionService/API/AuctionService.API.csproj", "AuctionService/API/"]
COPY ["AuctionService/Application/AuctionService.Application.csproj", "AuctionService/Application/"]
COPY ["AuctionService/Domain/AuctionService.Domain.csproj", "AuctionService/Domain/"]
COPY ["AuctionService/Infrastructure/AuctionService.Infrastructure.csproj", "AuctionService/Infrastructure/"]
COPY ["Common/Domain/Common.Domain.csproj", "Common/Domain/"]
COPY ["Common/Application/Common.Application.csproj", "Common/Application/"]
COPY ["Common/Infrastructure/Common.Infrastructure.csproj", "Common/Infrastructure/"]

RUN dotnet restore "AuctionService/API/AuctionService.API.csproj"

COPY . .

WORKDIR "/src/AuctionService/API"
RUN dotnet publish "AuctionService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "AuctionService.API.dll"]
