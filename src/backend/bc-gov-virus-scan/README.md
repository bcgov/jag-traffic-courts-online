
![Image of ClamAV](https://www.clamav.net/assets/clamav-trademark.png)
# ClamAV [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE) [![Lifecycle:Stable](https://img.shields.io/badge/Lifecycle-Stable-97ca00)](https://github.com/bcgov/repomountie/blob/master/doc/lifecycle-badges.md)


ClamAV® is an open source antivirus engine for detecting trojans, viruses, malware & other malicious threats.

# Virus Scan API

![image](https://user-images.githubusercontent.com/1844480/214445695-40a20423-27e6-4062-9cee-f03c737f80d9.png)

## Ping

![image](https://user-images.githubusercontent.com/1844480/214445891-0c16fa3f-3894-494f-8496-ac878e7d0666.png)

## Scan

![image](https://user-images.githubusercontent.com/1844480/214445980-a214c6bc-6d5e-42fe-bd9c-f8ec1922dc65.png)

### Large Files

This API uses ClamAV's INSTREAM command to scan files for viruses. ClamAV limits the maximum stream size to 25M.
The maximum can be changed by setting [StreamMaxLength](https://github.com/Cisco-Talos/clamav/blob/ebe59ef7dd31ab5c00071d3eee6f11fed1a00fd2/etc/clamd.conf.sample#L133)
configuration setting in ClamAV.

.NET sets the maximum allowed size of any request body in bytes. The default is 30,000,000 bytes, which is approximately 28.6MB. This can be changed using the `Kestrel:Limits:MaxRequestBodySize` configuration setting. See [Maximum request body size](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-7.0)

To be able to scan larger files, both these limits need to be adjusted.

TIP: Do not attempt to post a large file in one chunk. This service supports [chunked transfer encoding](https://en.wikipedia.org/wiki/Chunked_transfer_encoding).



## Version

![image](https://user-images.githubusercontent.com/1844480/214446050-7f94f633-730d-44a7-8cf5-6cd48568c2d8.png)
