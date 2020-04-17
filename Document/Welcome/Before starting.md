# Before starting

Be sure to read this before you begin to connect the service APIs for Aiursoft. Some parameters are described here.

## Basic API format

Aiursoft all services are using the following API protocol:

* HTTP
* WebSocket

其中，Aiursoft的HTTP协议具有下列特点:

* All services are directly opened to the entire Internet.
* All communication forced using trusted `HTTPS` certs.
* All protocol supports `HTTP/2`.
* All APIs' response type is `Json`.
* All response is encoded with `utf-8`.
* Everything respect English.
* All APIs' input format is `x-www-form-urlencoded`.

Aiursoft所有HTTP通讯的返回值中，都具有两个参数，也就是`code`和`message`。例如：

```json
{
    "code": 0,
    "message": "You have successfully created a message at channel:2!"
}
```

其中，`code`代表错误码，`message`代表基本提示信息。

在不同情况下，返回值的形态、结构**可能会发生变化**。但是都会包含这两个属性。因此建议开发者在任何情况下都应当优先检查`code`，仅在`code`符合期待后再进行下一步操作。若`code`是不期待的返回值，则应当进行处理，并在日志中记录`message`的值。

## 示例API

### 检查更新

请求地址：

    https://api.aiursoft.com/

方法

    HTTP GET

接口说明：

    本接口可以检查当前用户状态、服务器时钟、显示语言。

返回值示例：

```json
{
    "serverTime": "2018-03-12T10:29:31.2865921+08:00",
    "signedin": true,
    "local": "en",
    "user": {
        "id": "a75dff34-35d0-451d-bae3-af3c206bbc6b",
        "email": "anduin@aiursoft.com",
        "emailConfirmed": false,
        "bio": null,
        "nickName": "Anduin Xue",
        "sex": null,
        "headImgUrl": "https://oss.aiursoft.com/UsersIcon/9XNNIJF9RB.jpg",
        "preferedLanguage": "en",
        "accountCreateTime": "2018-02-09T15:09:23.1468265"
    },
    "code": 0,
    "message": "Server started successfully!"
}
```

## 常见错误分析

| 错误代码        | 错误说明    |  解决方案  |
|--|--|--|
|0|  成功完成了请求  |   不需要进行修正
|-1|   密钥错误  | 检查是否传入了合法的密钥
|-2|   请求被挂起  | 已经有一个相同意义的操作正在执行。请稍后再试。
|-3|   需要注意的警告  | 操作已经完成但是仍然需要注意。请阅读message参数值
|-4|   未找到  | 操作的目标对象不存在。请确认目标存在
|-5|   服务器崩溃  | 服务器未知错误。请向服务器团队提交反馈
|-6|   已经执行过了  | 已经有一个相同意义的操作已经执行完成。不需要进一步解决。
|-7|   没有足够的资源 | 目前可提供的资源无法满足操作。请检查请求的合理性。
|-8|   未授权 | 用户无法通过认证或没有进行该操作的权限。请确保用户的权限正常。
|-10|  输入值类型不符合规范 | 缺少参数，或传入的参数不符合规范。检查参数。
|-11|  超时 | 请求在处理中等待了很久而无法得到响应。请向服务器团队提交反馈。
