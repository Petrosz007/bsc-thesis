FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder

RUN mkdir /work
WORKDIR /work

COPY ./IWA_Backend.API/IWA_Backend.API.csproj ./
RUN dotnet restore

COPY ./IWA_Backend.API ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine

ENV IWA_AvatarDir ./AvatarData
EXPOSE 5000

RUN apk add --no-cache netcat-openbsd

WORKDIR /app
COPY --from=builder /work/out .

RUN mkdir AvatarData && \
    cp Avatars/default.jpg AvatarData/default.jpg

COPY docker/docker-entrypoint.sh docker/wait-for.sh /
RUN chmod +x /docker-entrypoint.sh /wait-for.sh

ENTRYPOINT [ "/docker-entrypoint.sh" ]
