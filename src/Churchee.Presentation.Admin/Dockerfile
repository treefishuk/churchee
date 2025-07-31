#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Churchee.Presentation.Admin/Churchee.Presentation.Admin.csproj", "src/Churchee.Presentation.Admin/"]
COPY ["src/Churchee.Data.EntityFramework/Churchee.Data.EntityFramework.csproj", "src/Churchee.Data.EntityFramework/"]
COPY ["src/Churchee.Common/Churchee.Common.csproj", "src/Churchee.Common/"]
COPY ["src/Churchee.Module.Identity/Churchee.Module.Identity.csproj", "src/Churchee.Module.Identity/"]
COPY ["src/Churchee.Module.UI/Churchee.Module.UI.csproj", "src/Churchee.Module.UI/"]
COPY ["src/Churchee.Module.Tenancy/Churchee.Module.Tenancy.csproj", "src/Churchee.Module.Tenancy/"]
RUN dotnet restore "./src/Churchee.Presentation.Admin/./Churchee.Presentation.Admin.csproj"
COPY . .
WORKDIR "/src/src/Churchee.Presentation.Admin"
RUN dotnet build "./Churchee.Presentation.Admin.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Churchee.Presentation.Admin.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Churchee.Presentation.Admin.dll"]