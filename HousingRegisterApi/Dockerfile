FROM mcr.microsoft.com/dotnet/sdk:3.1

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY */HousingRegisterApi/HousingRegisterApi.csproj ./
COPY /NuGet.config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./HousingRegisterApi.csproj

# Copy everything else and build
COPY . ./

RUN dotnet build -c Release -o out ./HousingRegisterApi.csproj
RUN dotnet publish -c Release -o out ./HousingRegisterApi.csproj

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/HousingRegisterApi.dll
