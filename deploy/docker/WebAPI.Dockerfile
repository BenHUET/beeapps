FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG APP_NAME
ENV NAME=$APP_NAME

WORKDIR /src
COPY ["./src/webapis/BeeApps.$NAME.WebAPI/BeeApps.$NAME.WebAPI.csproj", "./BeeApps.$NAME.WebAPI/"]
COPY ["./src/webapis/BeeApps.Common.WebAPI/BeeApps.Common.WebAPI.csproj", "./BeeApps.Common.WebAPI/"]
RUN dotnet restore "./BeeApps.$NAME.WebAPI/BeeApps.$NAME.WebAPI.csproj"

COPY ./src/webapis/BeeApps.$NAME.WebAPI ./BeeApps.$NAME.WebAPI
COPY ./src/webapis/BeeApps.Common.WebAPI ./BeeApps.Common.WebAPI

WORKDIR "/src/BeeApps.$NAME.WebAPI/"
RUN dotnet build "./BeeApps.$NAME.WebAPI.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "BeeApps.$NAME.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
ARG APP_NAME
ENV NAME=$APP_NAME
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet BeeApps.$NAME.WebAPI.dll