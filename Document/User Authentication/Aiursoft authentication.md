# Use Aiursoft authentication gateway in your application

Brief steps:

* Create an App
* Redirect the user
* Get the code
* Get an access token
* Transfer the code to user's openId
* Get user info from the user's openId (Optional)

## Create an App

Sign in [Aiursoft Developer Center](https://developer.aiursoft.com).

In **dashboard**, click the **Create app** button and name your new app.

Copy your **App Id** and **App Secret**.

### Permission settings

* Check: **Enable OAuth** in **OAuth Settings** page.
* Check: **View user's basic identity info** permission in **Permissions** page.


## Redirect the user

When the user requires login via Aiursoft authentication, redirect the user to the authentication page.

Redirect the user to: [OAuth page](./OAuth.md#Authorize)

## Get the code

After successfully authenticate the user, we will redirect the user back with:

* Code
* State

Like:

> https://yourapp.com/sign-in-aiursoft?code=12345&state=aaaaa

State is the value you passed in the last step. **Code** is what you need.

## Get an access token

You need an access token to authenticate your app.

Get one [here](../App%20Authentication/API.md#AccessToken) with your app id and app secret.

## Transfer the code to user's open ID

Code is one time used. Can only be used in 5 minutes. It doesn't represent the user's identity.

Open ID is static. One user can only have one unique open Id. It never change.

To get open ID from code, call API [here](./Account.md#CodeToOpenId).

Now you can focus on your own logic.

## Get user detailed information from open ID (Optional)

When you get the user's open ID, and the user has granted that you can view his profile, call this API to get detailed information:

[Get user information](./Account.md#UserInfo)

And you have got user's information and can focus on your own logic.
