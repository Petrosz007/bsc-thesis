FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder

RUN mkdir /app
WORKDIR /app

COPY ./IWA_Backend.API/IWA_Backend.API.csproj ./
RUN dotnet restore

COPY ./IWA_Backend.API ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0

EXPOSE 5000

WORKDIR /app
COPY --from=builder /app/out .

ENTRYPOINT [ "dotnet", "IWA_Backend.API.dll" ]
