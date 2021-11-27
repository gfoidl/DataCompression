# .NET Core 6 SDK + gnuplot 5

FROM    mcr.microsoft.com/dotnet/sdk:6.0

LABEL   org.opencontainers.image.source https://github.com/gfoidl/DataCompression

RUN     apt update \
&&      apt install -y gnuplot \
&&      rm -rf /var/lib/apt/lists/* /tmp/*
