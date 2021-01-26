# Use Aiursoft authentication in your web application

Brief steps:

* Create an App
* Redirect the user
* Get the `Code`
* Get an `Access token`
* Transfer the `Code` to user's `Open Id`
* Get user info from the user's `Open Id` (Optional)

## Create an App

Sign in [Aiursoft Developer Center](https://developer.aiursoft.com).

In **dashboard**, click the **Create app** button and name your new app.

Copy your **App Id** and **App Secret**.

### Permission settings

* Check: **Enable OAuth** in **OAuth Settings** page.
* Check: **View user's basic identity info** permission in **Permissions** page.


## Redirect the user

When the user requires login via Aiursoft authentication, redirect the user to the authentication page.

Redirect the user to the OAuth page.

**Do NOT call this like an API! This is a web page!**


<h3 id="Authorize"><span class="badge badge-pill badge-success">GET</span>  Authorize </h3>

Request path:

<p><kbd>https://gateway.aiursoft.com/OAuth/Authorize</kbd></p>

Request example:

<pre>
<code class="hljs pf">
https://gateway.aiursoft.com/OAuth/Authorize?appid=yourappid&amp;redirect_uri=yourredirect_uri&amp;state=yourstate
</code>
</pre>

Request arguments:

| Name          | Required     | Type    |
|---------------|--------------|---------|
| appid         | Required     | Text    |
| redirect_uri  | Required     | Text    |
| state         | Not required | Text    |
| force-confirm | Not required | Boolean |
| try-auth      | Not required | Boolean |

## Get the code

After successfully authenticate the user, we will redirect the user back with:

* Code
* State

Like:

> https://yourapp.com/sign-in-aiursoft?code=12345&state=aaaaa

`State` is the value you passed in the last step. 

**Code** represents the identity of the user.

## Get an access token

You need an access token to authenticate your app.

Read [here](../App%20Authentication/API.md#AccessToken) to get your app id and app secret.

## Transfer the code to the user's open ID

`Code` is one time used. Can only be used in 5 minutes. It doesn't represent the user's identity.

Open ID is always the unique ID of that user. It never change.

To get the `Open ID` from `Code`, call our API [here](./Account.md#CodeToOpenId).

Now you can focus on your own logic.

## Get user detailed information from open ID (Optional)

When you get the user's open ID, and the user has granted that you can view his profile, call this API to get detailed information:

[Get user information](./Account.md#UserInfo)

And you have got user's information and can focus on your own logic.
