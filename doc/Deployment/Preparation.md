# 部署的软硬件准备

是的，整套Aiursoft微服务平台都可以完整的不依赖公共网络，而完全部署在用户的私有云中。

本章节将详细介绍Aiursoft微服务平台的本地部署步骤。

## 确保一些资源

1. 确保你已经具有了一个域名，例如`example.com`。请访问它的DNS设置，确保你可以增加A记录。
2. 确保你已经具有了一台全新的`Windows Server 2016`英文版服务器，并具有一个你能够访问的到的IP地址。

### 域和DNS准备（可选）

1. 额外准备两台全新的`Windows Server 2016`服务器
2. 分别在两台服务器上部署Active Directory and Domain Service
3. 配置Active Directory。域的名称不得和你所拥有的域名相同，建议命名为`example.local`
4. 将所有准备安装数据库的服务器和准备用于运行Aiursoft平台的服务器加入域

### 服务器准备

1. 开启其防火墙，并开放80、443端口
2. 新建一个用于管理员操作的用户，例如`aiuradmin`。将其加入到`Administrators`用户组中（如果配置了域，则需要加入`Domain Admins`）
3. 新建一个用于运行Aiursoft微服务平台的用户。例如`runtime`，将其加入`Users`用户组中。（如果配置了域，则需要加入`Domain Users`）
4. 禁用`Administrator`用户
5. 开启UAC设置（官方的Windows Server中默认是开启的）
6. 登录到`aiuradmin`用户
7. 增加环境变量`ASPNETCORE_ENVIRONMENT`，设置值为`Production`
8. 重新启动服务器
9. 为服务器添加功能：Web服务器（IIS）

### 数据库准备

1. 下载最新版本的SQL Server 2017 Community或Enterprise版本[安装程序](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
2. 运行安装程序，将安装实例名调整为默认实例
3. 安装的模组选择SQL Server数据库引擎
4. 在SQL Server登录中，选择混合身份验证
5. 在SQL Server系统管理员中，增加`aiuradmin`和`runtime`
6. 确保SQL Server服务能够正常运行
7. 在防火墙开放1433端口（如果需要）

### DNS准备

1. 在DNS服务器设置中，为`example.com`域名解析到Web服务器
2. 将`api.example.com`域名解析到Web服务器
3. 将`developer.example.com`域名解析到Web服务器
4. 将`oss.example.com`域名解析到Web服务器
5. 将`stargate.example.com`域名解析到Web服务器
6. 将`www.example.com`域名解析到Web服务器
7. 将`wiki.example.com`域名解析到Web服务器
8. 将`cdn.example.com`域名解析到Web服务器

### 证书准备

1. 为`api.example.com`购买受信任的证书
2. 为`developer.example.com`购买受信任的证书
3. 为`oss.example.com`购买受信任的证书
4. 为`stargate.example.com`购买受信任的证书
5. 为`www.example.com`购买受信任的证书
6. 为`wiki.example.com`购买受信任的证书
7. 为`cdn.example.com`购买受信任的证书

或

1. 购买`*.eample.com`通配符证书

### 依赖项准备

1. 在Aiursoft服务器上安装最新版本[Git](https://git-scm.com)
2. 在Aiursoft服务器上安装最新版本[Nodejs](https://nodejs.org)和`npm`
3. 在Aiursoft服务器上安装最新版本[`Visual Studio Code`](https://code.visualstudio.com)
4. 在Aiursoft服务器上安装最新版本[.NET Core SDK](https://www.microsoft.com/net)
5. 在Aiursoft服务器上安装[.NET Core Hosting Bundle](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.1&tabs=aspnetcore2x#install-the-net-core-hosting-bundle)
6. 重新启动服务器

### 其它警告

1. 警告：历史上曾因使用非隔离的用户运行Aiursoft平台产生错误！用户在通过图形界面注销时，会影响到平台。
2. 警告：任何情况下，不应当使用管理员权限运行Aiursoft平台！
3. 警告：证书必须是受信任的。因为不但用户强制要求通过HTTPS访问，平台内部通讯也需要检查证书。不受信任的证书会导致平台内部通讯！
4. 注意：如果无法支付昂贵的证书费用，可以自己为自己颁发证书，并在Aiursoft Web服务器上和需要测试的计算机上将自己设置为受信任的证书办法机构。

#### 恭喜！至此，你已经成功准备好了需要运行Aiursoft的全部软件、硬件环境！