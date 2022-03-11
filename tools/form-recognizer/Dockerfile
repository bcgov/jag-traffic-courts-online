FROM mcr.microsoft.com/azure-cognitive-services/form-recognizer/layout

COPY /shared-src /shared-src
COPY /init.sh /init.sh

ENTRYPOINT ["/bin/sh", "-c", "/init.sh && /init-with-cmd-args.sh"]
