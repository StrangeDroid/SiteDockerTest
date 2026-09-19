# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/BlazorApp1

# Copy only the CSProj and restore dependencies
COPY BlazorApp1/BlazorApp1.csproj ./
RUN dotnet restore

# Copy the rest of your code and publish
COPY BlazorApp1/ ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Listen on all interfaces for both ports
ENV ASPNETCORE_URLS="http://0.0.0.0:5273;http://0.0.0.0:7041"

# Copy the published output
COPY --from=build /app/publish ./

# Expose container ports (for documentation; actual mapping happens at `docker run`)
EXPOSE 5273
EXPOSE 7041

# Start the app
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
