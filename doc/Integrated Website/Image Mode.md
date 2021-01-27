# Image mode

If you are trying to load a photo in `Open mode`, it will be switched to `Image mode`.

## Trigger

We treat files with the following extensions to switch to `Image mode`:

* jpg
* png
* bmp
* jpeg

## EXIF clearing

Probe will always return the image after clearing it's EXIF data.

This feature is forcibly enabled and can not be shutdown.

## Compression

### Source image

Do not append anything to view the source image.

https://probe.aiursoft.com/download/open/aiursoft-app-icons/8c716dde448109f940929844b41a39a0/e0abd2118b9f4381bb490211c1d9e722.png

![Source](https://probe.aiursoft.com/download/open/aiursoft-app-icons/8c716dde448109f940929844b41a39a0/e0abd2118b9f4381bb490211c1d9e722.png)

### Scale image

Append argument `w` to specify the width to scale.

https://address.png?w=10

![Source](https://probe.aiursoft.com/download/open/aiursoft-app-icons/8c716dde448109f940929844b41a39a0/e0abd2118b9f4381bb490211c1d9e722.png?w=10)

### Stretch image as a square

Append `w` and `square=true`


https://address.png?w=10

![Source](https://probe.aiursoft.com/download/open/aiursoft-app-icons/8c716dde448109f940929844b41a39a0/e0abd2118b9f4381bb490211c1d9e722.png?w=10&square=true)

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