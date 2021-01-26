# Aiursoft integrated site Service - Probe

Probe is the basic storage service for all Aiursoft apps.

## Structure

Probe structure:

* Probe
  * App A
    * Site A
      * Folder A
        * File A

## Use Probe

If you plan to use Probe in your project, please follow the steps below

* [Integrate Aiursoft App Authentication](../App%20Authentication/What%20is%20app%20authentication.md)
* [Access Probe API](./Advanced%20API.md)

## Modes

There are 3 modes when the user is trying to download a file from Probe.

* Open mode.
* Download mode.
* Video mode.

The modes are determined by the URL. 

* If it starts with: `/download/open`, it's in the open mode.
* If it starts with: `/download/file`, it's in the download mode.
* If it starts with: `/download/video`, it's in the video mode.

### Open mode

The open mode will try to return the file with a supported MIME type. So you can preview the file in your browser.

If we can't detect the MIME type, the open mode will be switched to download mode automatically.

If you are trying to load a photo in `Open mode`, it will be switched to `Image mode` to do some compression and EXIF clearing. Read more [here](./Image%20Mode).

### Download mode

Probe will always suggest the browser to download the file directly.

### Video mode

Probe will open a video player in a Web page and try to play the file as a video.

## Size limitation and pricing

Probe is currently free. But maybe it will not be free some day.

Possible price:

> 1$ / GB / Month.

We are still investigating this.
