# Aiursoft Nexus

[![Build status](https://aiursoft.visualstudio.com/Star/_apis/build/status/Nexus%20Build)](https://aiursoft.visualstudio.com/Star/_build/latest?definitionId=5)
[![Issues](https://img.shields.io/github/issues/AiursoftWeb/Nexus.svg)](https://github.com/AiursoftWeb/Nexus/issues)

Aiursoft micro-services platform. Powered by ASP.NET Core.

## What is Nexus

Nexus is not a framework, not a tool, nor a library. It was just a project, runs in the application level, using micro-services design and powers all Aiursoft applications.

Nexus provides many useful services and simplifies web application development.

Nexus provides a central platform for an entire enterprise. So the enterprise can focus on business app development.

## Features

* Integrated user authentication https://gateway.aiursoft.com (OAuth, Social authenticate, password reset)
* Integrated app authentication https://archon.aiursoft.com (Token assign, token validation, app authorization)
* Account management center https://account.aiursoft.com (Profile editting, Two-FA, social account, switch email)
* Developer center https://developer.aiursoft.com (App creation and permission management, basic debugging tools)
* File storage system https://probe.aiursoft.com (Compress, Clear EXIF, customize domain-name)
* Message queue https://stargate.aiursoft.com (Auto message clear, WebSocket push, channel authentication)
* Home web page https://www.aiursoft.com (Terms, privacy)
* Wiki center https://wiki.aiursoft.com (Read document)
* Cross-platform deployment (Supports Windows, Linux, and docker)

## Layers

Nexus code structure has 4 layers:

* Abstract layer - Contains basic interfaces and code style abstracts.
  * [Scanner.Abstract](./Scanner.Abstract/Readme.md) - Abstract layer for `Aiursoft.Scanner`.
* Basic tools layer - Contains basic tools for C#.
  * [Handler](./Handler/Readme.md) - An error handle framework for ASP.NET Core
  * [DocGenerator](./DocGenerator/Readme.md) - A basic API document generator for ASP.NET Core applications.
  * [Scanner](./Scanner/Readme.md) - An Automatic dependencies management system for ASP.NET Core.
  * [XelNaga](./XelNaga/Readme.md) - Some other C# tools.
* SDK layer
  * SDK - Contains Aiursoft interface defination and abstract.
  * Pylon - ASP.NET Core toolset. Contains tools for building web apps for Aiursoft.
* Web apps layer

Nexus architect has three layers for web applications:

* Basic Services
* Infrastructures
* Applications

### Basic Services

The basic services are used to support the operation of the entire platform. It is the basis for the expansion of the subsequent infrastructure and user services throughout the platform. The basic services mainly include the user's authentication, authorization, application authentication, authorization, and allow the user's self-registration and self-registration of the application. At the same time, the underlying service is also responsible for the user's underlying data and the underlying data of the application, as well as the user's set of credentials and the set of credentials for the application.

### Infrastructures

Infrastructure refers to some common software tools that the platform can provide on an infrastructure-based basis. Such software tools are similar to static file storage, object storage, CDN, message queues, caching, and the like. Such services need to be registered in the underlying service and use the application authentication service and the application authorization service to check if the visitor has permission to invoke the infrastructure.

### Applications

Before discussing the user service layer, all of the underlying services and infrastructure we discussed were oriented toward other subservices rather than user-oriented, but only provided data interfaces for other service calls. Obviously, as a mature platform, developers and users should not implement the registration of services and the management of permissions by calling interfaces. Therefore, we need to develop additional programs that provide such a user-oriented terminal that allows users to operate through a concise interface and logic.

In addition to the user interface that OAuth itself needs to provide login, this layer should also provide a more basic service in the microservice platform. According to the results of the needs analysis, there are account management services and developer center services in this layer. Both services rely on the infrastructure of the previous layer and provide user-oriented functionality.

Any high-level service is built on the micro-service platform. But the requirements they solve are actually based on the decisions of the companies that use the microservices platform. In other words, regardless of the company's decision-making to build search engines, forums, company homepages, feedback centers, recruitment centers, etc., the platform should be able to support. But the entire process of this part should follow the process followed by a separate software system.

## How to run

Running the entire micro-service platform is very very complicated and requires a lot of computing resources. Before you try to run the platform, we strongly suggest running only one specific service. Running one service is very easy and simple.

For running the platform, The document is [here](https://wiki.aiursoft.com/ReadDoc/Deployment/Getting%20Started.md).

## How to contribute

The document is [here](https://wiki.aiursoft.com/ReadDoc/Getting%20Involed/How%20to%20contribute.md).

## View running services in production

If you wanna try the platform, all independent services are deployed under the following link. Click any of it allows you to try viewing a service.

* [Account](https://account.aiursoft.com)
* [API](https://api.aiursoft.com)
* [Archon](https://archon.aiursoft.com)
* [Colossus](https://colossus.aiursoft.com)
* [Developer](https://developer.aiursoft.com)
* [EE](https://ee.aiursoft.com)
* [Stargate](https://stargate.aiursoft.com)
* [Wiki](https://wiki.aiursoft.com)
* [WWW](https://www.aiursoft.com)
