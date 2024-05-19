#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app
ENV TZ="America/Sao_Paulo"
ARG RABBITMQ_HOSTNAME
ARG RABBITMQ_USERNAME
ARG RABBITMQ_PASSWORD
ARG REDIS_HOSTNAME
ARG REDIS_PASSWORD
ARG MARIADB_SERVER
ARG MARIADB_DATABASE
ARG MARIADB_USER
ARG MARIADB_PASSWORD
ENV RABBITMQ_HOSTNAME=${RABBITMQ_HOSTNAME}
ENV RABBITMQ_USERNAME=${RABBITMQ_USERNAME}
ENV RABBITMQ_PASSWORD=${RABBITMQ_PASSWORD}
ENV REDIS_HOSTNAME=${REDIS_HOSTNAME}
ENV REDIS_PASSWORD=${REDIS_PASSWORD}
ENV MARIADB_SERVER=${MARIADB_SERVER}
ENV MARIADB_DATABASE=${MARIADB_DATABASE}
ENV MARIADB_USER=${MARIADB_USER}
ENV MARIADB_PASSWORD=${MARIADB_PASSWORD}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TechChallenge.Worker/TechChallenge.Worker.csproj", "TechChallenge.Worker/"]
RUN dotnet restore "./TechChallenge.Worker/TechChallenge.Worker.csproj"
COPY . .
WORKDIR "/src/TechChallenge.Worker"
RUN dotnet build "./TechChallenge.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechChallenge.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechChallenge.Worker.dll"]