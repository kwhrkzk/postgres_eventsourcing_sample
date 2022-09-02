FROM mcr.microsoft.com/dotnet/sdk:7.0

USER root
COPY ./docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

ENTRYPOINT [ /docker-entrypoint.sh ]
