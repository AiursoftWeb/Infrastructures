# Probe advanced usage

## Clear EXIF

All images' EXIF info will be cleared when opening in browser.

Download feature will always return the source image with EXIF.

This feature is forcibly enabled and can not be shutdown.

## Probe image compressing pattern

### Source image

Do not append anything.

https://probe.aiursoft.com/download/open/kahla-user-files-staging/conversation-2/2020-04-13/clipboardImg_1586748117463.png

### Scale image

Only append `w`.

https://probe.aiursoft.com/download/open/kahla-user-files-staging/conversation-2/2020-04-13/clipboardImg_1586748117463.png?w=128

### Square image

Append `w` and `square=true`

https://probe.aiursoft.com/download/open/kahla-user-files-staging/conversation-2/2020-04-13/clipboardImg_1586748117463.png?w=128&square=true

### Number indicator

w can only be one of the following numbers:

```
0(Source)
1
2
4
8
16
...
8192
```

More than 8192 will be floor to 8192.

## Download

The file download feature does **NOT** support image compressing and EXIF clearing.

## Open vs Download

* The route `/download/open` will try to open the file. If fail, then download.
* The route `/download/file` will download the source file by force.

## Short domain (Under development)

For a file in Probe:

> https://probe.aiursoft.com/Download/Open/{SiteName}/{FilePath}

Can be download via:

> https://{SiteName}.aiursoft.io/{FilePath}

For example, the file in Probe with address:

> https://probe.aiursoft.com/Download/Open/mysite/512x512.png

Can be download via:

> https://mysite.aiursoft.io/512x512.png

## Upload token and download token

Under consideration.
