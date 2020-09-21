# Aiursoft Intern Project

Aiursoft Intern Project，指的是面向中国大陆范围内的学生开放的，关于Aiursoft的一些开放性问题的解决方案征集。

Aiursoft Intern Project 是完全由个人的兴趣爱好驱动，以无回报、提升个人知识水平能力和贡献社区为目标、并且能够接触到实际生产的真实业务需求的机会。

## 奖励

参加 Aiursoft Intern Project 将有机会获得：

* Aiursoft Intern Project 参与证书
* 至少价值 500 元的 Steam\Battle.net\Origin\Uplay\XBox 平台游戏
* Microsoft Office 365 开发者版（含2TB OneDrive）
* Aiursoft 文化衫和周边
* GitHub 组织成员权限

另外，Aiursoft 允许并鼓励你将 Aiursoft Intern Project 作为你的社区实习、课题设计、毕业设计题目，并愿意为你提供支持材料。

## 要求

参加人

* 参加者必须是位于中国大陆的或国籍为中国大陆的学生。
* 参加者必须同意 [Aiursoft 许可协议](https://www.aiursoft.com/docs/terms)。
* 单一题目的完成期限不可超过四个月。

## 报名方式

发送Email至：[officials@aiursoft.com](mailto:officials@aiursoft.com)。

Email中请注明：

* 你的 GitHub 用户名
* 你的 Aiursoft 账户名
* 你选择的题目
* 计划起止时间

## 题目列表

### 1. 可能认识的人

Kahla 是 Aiursoft 的业务系统之一。它的数据库中，存储了一些用户和一些会话。会话中有若干个参与的用户（2-无穷个）。换句话说，每一个用户，都有一个包含自己的会话列表。同时，Aiursoft 能够取得每个会话内每个人的发言次数和时间。（无法取得发言内容） 我们需要构建一个解决方案，能够针对特定用户，求出他最可能认识的人，以作为好友推荐数据。

* 上述数据均存储在SQL Server中。
* 你必须提供一个服务，其输入值为当前用户ID，返回值为一个集合，按照他最可能认识的人排序。
* 针对样例数据，必须能够在双核2GB的Linux服务器上1秒内返回。

**Aiursoft 将提供样例数据。**

-------

### 2. Call on Web

Kahla 是 Aiursoft 的业务系统之一。它的前端由Angular构建，而面向了桌面 (Electron) 和PWA平台。它的后端是由C# + ASP.NET Core构建。而你需要将其整合 Web-rtc 技术，构建一个解决方案，允许2个或N个用户建立实时的语音通话，以实现多人语音会议。

* 你需要构建一个 Demo 项目，并将其部署到你自己的服务器上。
* 你的Demo项目必须包含 Angular 开发的前端和 ASP.NET Core 开发的后端。
* 你的Demo项目必须能够完成一次至少2人的实时语音通话演示，且接入者存在于NAT设备之后。

-------

### 3. Micro-service to K8S

Infrastructures 是 Aiursoft 的底层基础结构系统。它由 ASP.NET Core 构建，并且本身的设计中已经被拆分成了数个小服务，它们互相通过 HTTP 调用、组合来形成对外的整体服务。我们分别验证了几乎任何一个小服务都可以面向容器。但是，我们期待能够更多的享受微服务和容器带来的便利。

* 你必须开发脚本或配置文件，能够在不与 Aiursoft 任何现有服务通信的情况下，快速搭建 Aiursoft Infrastructures。
* 你的脚本或配置文件必须能够将 Aiursoft Infrastructures 的所有服务都安装在一个特定的 Kubernetes 集群中
* 你的方案必须尽可能廉价的解决自动化 HTTPS 证书续期问题。（搭建后，在云计算上最低花费不超过 150 USD/月）
* 你的方案必须解决 SQL Server 持久化和 Probe 的文件系统持久化。

### 4. See latest one

系统 K 使用了 WebSocket 将事件由服务器发送至客户端。每次有新事件时，客户端都会收到而将其快速展示。这些事件能够形成一个集合：“事件集”。我们期待在服务器和客户端，事件集应当尽可能相同。但是，遗憾的是：在客户端网络不稳定的情况下，WebSocket有一定几率会丢失消息。那么客户端应当在它自己网络联通后，尽快拉取到它丢失的消息，以实现集合的完全同步。同时，客户端可能会向事件集中增加消息。这会向服务器发送HTTP Post请求。即使服务器没有得到响应，客户端也需要将其插入到本地的事件集中暂存，以实现离线可用。在网络联通后，尽快与服务器的集合保持同步。

* 你必须开发基于 ASP.NET Core 的服务器端事件集同步SDK
* 你必须开发基于 Angular 的客户端事件集同步SDK
* 你必须构建一个 Demo，满足上述场景，并且尽可能节省流量
    

