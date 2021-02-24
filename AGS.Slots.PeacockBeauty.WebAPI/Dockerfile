#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AGS.Slots.MermaidsFortune.WebAPI/AGS.Slots.MermaidsFortune.WebAPI.csproj", "AGS.Slots.MermaidsFortune.WebAPI/"]
RUN dotnet restore "AGS.Slots.MermaidsFortune.WebAPI/AGS.Slots.MermaidsFortune.WebAPI.csproj"
COPY . .
WORKDIR "/src/AGS.Slots.MermaidsFortune.WebAPI"
RUN dotnet build "AGS.Slots.MermaidsFortune.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AGS.Slots.MermaidsFortune.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AGS.Slots.MermaidsFortune.WebAPI.dll"]