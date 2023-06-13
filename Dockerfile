FROM mcr.microsoft.com/dotnet/sdk:7.0 AS DacpacBuilder
WORKDIR /src
COPY ["DbSchema/*", "DbSchema/"]
RUN dotnet build "DbSchema/DbSchema.sqlproj"

FROM mcr.microsoft.com/mssql/server:2017-latest AS SqlBuilder
ARG DB_NAME
ARG ACCEPT_EULA
ARG SA_PASSWORD
ENV ACCEPT_EULA=${ACCEPT_EULA}
ENV SA_PASSWORD=${SA_PASSWORD}
WORKDIR /sql


# Root needed for apt-get install, switch to mssql after
#USER root

# Install Unzip
RUN apt-get update \
    && apt-get install unzip -y

# Install SQLPackage for Linux and make it executable
RUN wget -q -O sqlpackage.zip https://aka.ms/sqlpackage-linux \
    && unzip -qq sqlpackage.zip -d /sql/sqlpackage \
    && chmod +x /sql/sqlpackage/sqlpackage



COPY --from=DacpacBuilder /src/DbSchema/bin/Debug/DbSchema.dacpac /tmp/db.dacpac
#USER mssql
RUN ( /opt/mssql/bin/sqlservr & ) | grep -q "Service Broker manager has started" \
    && /sql/sqlpackage/sqlpackage /a:Publish \
        /TargetConnectionString:"Server=localhost;Initial Catalog=${DB_NAME};User ID=sa;Password=${SA_PASSWORD};Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;" \  
        /sf:/tmp/db.dacpac \
    && rm /tmp/db.dacpac \
    && pkill sqlservr


# Create the final image
FROM mcr.microsoft.com/mssql/server:2017-latest
COPY --from=SqlBuilder /var/opt/mssql/data/* /var/opt/mssql/data/

EXPOSE 1433

# USER root
# RUN chown -R mssql /var/opt/mssql/data
# USER mssql
