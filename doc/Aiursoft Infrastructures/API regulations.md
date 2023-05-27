# Before starting

Be sure to read this before you begin to connect the service APIs for Aiursoft. Some parameters are described here.

## Basic API format

Aiursoft all services are using the following API protocol:

* HTTP
* WebSocket

* All services are directly opened to the entire Internet.
* All communication forced using trusted `HTTPS` certs.
* All protocol supports `HTTP/2`.
* All APIs' response type is `Json`.
* All response is encoded with `utf-8`.
* Everything respect English.
* All APIs' input format is `x-www-form-urlencoded`.

All API response by Aiursoft must contains the following two arguments: `code` and `message`. For example:

```json
{
    "code": 0,
    "message": "You have successfully created a message at channel:2!"
}
```

The code is a number for state. And message is some information for caller.

For different, the response value might change. But always contains the two properties. So we suggest developers check the code first before getting other properties. And always log the message value if unexpected code.

## Example API

### Directory status API

Address

    https://directory.aiursoft.com/

Method

    HTTP GET

Description

    Get current user details and server time.

Return value example:

```json
{
    "serverTime": "2020-09-21T11:53:18.5895448Z",
    "signedIn": true,
    "local": "en",
    "user": {
        "emailConfirmed": true,
        "email": "anduin@aiursoft.com",
        "id": "6da0802e-18e4-4a76-99a5-47878dd7b8a5",
        "bio": "https://aka.ms/anduin",
        "nickName": "Anduin Xue",
        "sex": null,
        "iconFilePath": "usericon/2019-10-15/newi-small.png",
        "preferedLanguage": "en-US",
        "accountCreateTime": "2018-07-03T23:29:38.9450153Z"
    },
    "code": 0,
    "message": "Server started successfully!"
}
```

## Code states

| Code        | Description    |  solution  |
|--|--|--|
|0 | Request completed successfully | No correction required
|-1 | Wrong key. | Check whether a legal key is passed
|-2 | Request pending | An operation with the same meaning is already in progress. Please try again later.
|-3 | Cautions | The operation has been completed, but still needs attention. Read the value of the message parameter
|-4 | Not found | The target object of the operation does not exist. Please confirm that the target exists
|-5 | Server crash | Server unknown error. Please submit feedback to the server team
|-6 | Has been executed | An operation with the same meaning has been executed. No further resolution is needed.
|-7 | There are not enough resources | The available resources cannot meet the operation requirements. Please check the rationality of the request.
|-8 | Unauthorized | The user cannot pass the authentication or does not have the permission to perform the operation. Make sure that the user's permissions are normal.
|-10 | The input value type is invalid | The parameter is missing, or the parameter passed in does not conform to the specification. Check the parameters.
|-11 | Timeout | The request has been waiting for a long time in processing and cannot be responded. Please submit feedback to the server team.
