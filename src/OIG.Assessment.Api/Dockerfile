FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/OIG.Assessment.Api/OIG.Assessment.Api.csproj", "src/OIG.Assessment.Api/"]
COPY ["src/OIG.Assessment.Application/OIG.Assessment.Application.csproj", "src/OIG.Assessment.Application/"]
COPY ["src/OIG.Assessment.Domain/OIG.Assessment.Domain.csproj", "src/OIG.Assessment.Domain/"]
COPY ["src/OIG.Assessment.Infrastructure/OIG.Assessment.Infrastructure.csproj", "src/OIG.Assessment.Infrastructure/"]

RUN dotnet restore "src/OIG.Assessment.Api/OIG.Assessment.Api.csproj"

COPY . .

WORKDIR "/src/src/OIG.Assessment.Api"
RUN dotnet publish "OIG.Assessment.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OIG.Assessment.Api.dll"]

