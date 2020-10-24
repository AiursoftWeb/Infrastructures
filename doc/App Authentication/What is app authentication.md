# App authentication

There is two authentication system in Aiursoft.

* User Authentication.
* App Authentication.

User Authentication is only for users to enjoy our services. While app authentication is for developers to use our API.

## What is the Aiursoft App

Aiursoft App stands for an independent identity.

It is created and managed in [Aiursoft Developer Center](https://developer.aiursoft.com). 

After creating your app, you can get `App Id` and `App Secret`.

--------

The `App Id` is public. You can share it with every one or send it to the client-side.

The `App Secret` is confidential. Only save it to a safe key-vault and only use it to get a token.

## What is the Aiursoft App Token

Aiursoft App Token indicates that it has the permission to act as an app. It is confidential. Don't send it to the client-side. But you can pass it to other Aiursoft services.

Token has a limited life time. Usually 20 minutes. Save it to cache!

## How to use an app to get a token

The get a token, reference the app authentication api [here](./API.md).