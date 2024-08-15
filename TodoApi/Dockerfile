FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY *.csproj .
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
# copy and publish app and libraries
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
CMD ["dotnet", "TodoApi.dll"]