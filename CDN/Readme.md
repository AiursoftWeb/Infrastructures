# Aiursoft.CDN

[![Build Status](https://travis-ci.org/AiursoftWeb/CDN.svg?branch=master)](https://travis-ci.org/AiursoftWeb/CDN)

The place for shared static resources of Aiursoft web apps

## How to run

We **strongly recommend** running or modifying this app using Visual Studio Code

### Dependencies

* npm

### Bundle

Please excuse the following commands in the project folder:

    npm install
    npm run bundle

## Components

### AiurCore

AiurCore is for all Aiursoft apps. This contains some basic tools for Aiursoft front-end programs.

* jQuery
* Popper
* Bootstrap
* Font-awesome
* jquery.validate.js
* jquery.validate.unobtrusive.js
* Clipboard
* jquery-disable-with
* AiurCore

### AiurMarket

AiurMarket is for Aiursoft home pages. This will create beautiful landing page.

* AiurMarket

### AiurProduct

AiurProduct is for Aiursoft download pages.

* Primer
* AiurProduct

### AiurDashboard

AiurProduct is for Aiursoft dashboard pages.

* SB-Admin
* Datatable
* primer-markdown

## What is the relationship with other Aiursoft apps

All Aiursoft web applications with view shall put all those static files, like stylesheets or js files in our CDN server.

User get those files directy from CDN server.

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your personal workflow cruft out of sight.

We're also interested in your feedback for the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.