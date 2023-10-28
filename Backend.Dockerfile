FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# HTTP
EXPOSE 80

# HTTPS
EXPOSE 443

# Swagger
EXPOSE 7034

# Listening port (connection from frontend)
EXPOSE 5118

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SocialStoriesBackend/SocialStoriesBackend.csproj", "SocialStoriesBackend/"]
RUN dotnet restore "SocialStoriesBackend/SocialStoriesBackend.csproj"
COPY . .
WORKDIR "/src/SocialStoriesBackend"
RUN dotnet build "SocialStoriesBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialStoriesBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialStoriesBackend.dll"]
