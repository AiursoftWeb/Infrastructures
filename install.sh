aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

nexus_code="./Nexus"
nexus_path="/opt/apps/Nexus"
dbPassword=$(uuidgen)
userId=$(uuidgen)
developerAppId=$(uuidgen)
developerAppSecret=$(uuidgen)

archon_code="$nexus_code/src/WebServices/Basic/Archon"
gateway_code="$nexus_code/src/WebServices/Basic/Gateway"
developer_code="$nexus_code/src/WebServices/Basic/Developer"
observer_code="$nexus_code/src/WebServices/Infrastructure/Observer"
probe_code="$nexus_code/src/WebServices/Infrastructure/Probe"
stargate_code="$nexus_code/src/WebServices/Infrastructure/Stargate"
wrapgate_code="$nexus_code/src/WebServices/Infrastructure/Wrapgate"
www_code="$nexus_code/src/WebServices/Business/WWW"
wiki_code="$nexus_code/src/WebServices/Business/Wiki"
status_code="$nexus_code/src/WebServices/Business/Status"
account_code="$nexus_code/src/WebServices/Business/Account"
colossus_code="$nexus_code/src/WebServices/Business/Colossus"
wrap_code="$nexus_code/src/WebServices/Business/Wrap"
ee_code="$nexus_code/src/WebServices/Business/EE"

archon_path="$nexus_path/Archon"
gateway_path="$nexus_path/Gateway"
developer_path="$nexus_path/Developer"
observer_path="$nexus_path/Observer"
probe_path="$nexus_path/Probe"
stargate_path="$nexus_path/Stargate"
wrapgate_path="$nexus_path/Wrapgate"
www_path="$nexus_path/WWW"
wiki_path="$nexus_path/Wiki"
status_path="$nexus_path/Status"
account_path="$nexus_path/Account"
colossus_path="$nexus_path/Colossus"
wrap_path="$nexus_path/Wrap"
ee_path="$nexus_path/EE"

set_env()
{
    dist_path="$1"
    domain="$2"

    aiur text/edit_json "ConnectionStrings.DeveloperConnection" "https://developer.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ArchonConnection" "https://archon.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.GatewayConnection" "https://gateway.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.StargateConnection" "https://stargate.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ObserverConnection" "https://observer.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ProbeConnection" "https://probe.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.WrapgateConnection" "https://wrapgate.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.AccountPath" "https://account.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.UIPath" "https://ui.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.ColossusPath" "https://colossus.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.WrapPath" "https://wrap.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.EEPath" "https://ee.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.WikiPath" "https://wiki.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.WWWPath" "https://www.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.StatusPath" "https://status.$domain" $dist_path/appsettings.Production.json
}

add_service()
{
    name="$1"
    path="$2"
    dll="$3"
    domain="$4"

    port=$(aiur network/get_port)
    aiur services/register_aspnet_service $name $port $path $dll
    aiur caddy/add_proxy $name.$domain $port
}

replace_in_file()
{
    file="$1"
    from="$2"
    to="$3"
    cat $file | sed "s/$from/$to/g" | sponge $file
}

install_nexus()
{
    if [[ $(curl -sL ifconfig.me) == "$(dig +short $(uuidgen).$1)" ]]; 
    then
        echo "IP is correct."
    else
        echo "You need to create a wildcard DNS record *.$1 to $(curl -sL ifconfig.me)!"
        return 9
    fi

    curl -sL https://github.com/AiursoftWeb/AiurUI/raw/master/install.sh | bash -s ui.$1

    aiur system/set_aspnet_prod
    aiur install/dotnet
    aiur install/jq
    aiur install/sql_server
    aiur mssql/config_password $dbPassword

    aiur git/clone_to AiursoftWeb/Nexus $nexus_code
    dotnet restore ./Nexus/Nexus.sln
    aiur dotnet/publish $archon_path $archon_code/"Aiursoft.Archon.csproj"
    aiur dotnet/publish $gateway_path $gateway_code/"Aiursoft.Gateway.csproj"
    aiur dotnet/publish $developer_path $developer_code/"Aiursoft.Developer.csproj"
    aiur dotnet/publish $observer_path $observer_code/"Aiursoft.Observer.csproj"
    aiur dotnet/publish $probe_path $probe_code/"Aiursoft.Probe.csproj"
    aiur dotnet/publish $stargate_path $stargate_code/"Aiursoft.Stargate.csproj"
    aiur dotnet/publish $wrapgate_path $wrapgate_code/"Aiursoft.Wrapgate.csproj"
    aiur dotnet/publish $www_path $www_code/"Aiursoft.WWW.csproj"
    aiur dotnet/publish $wiki_path $wiki_code/"Aiursoft.Wiki.csproj"
    aiur dotnet/publish $status_path $status_code/"Aiursoft.Status.csproj"
    aiur dotnet/publish $account_path $account_code/"Aiursoft.Account.csproj"
    aiur dotnet/publish $colossus_path $colossus_code/"Aiursoft.Colossus.csproj"
    aiur dotnet/publish $wrap_path $wrap_code/"Aiursoft.Wrap.csproj"
    aiur dotnet/publish $ee_path $ee_code/"Aiursoft.EE.csproj"

    aiur dotnet/seeddb $gateway_path "Gateway" $dbPassword
    aiur dotnet/seeddb $developer_path "Developer" $dbPassword
    aiur dotnet/seeddb $observer_path "Observer" $dbPassword
    aiur dotnet/seeddb $probe_path "Probe" $dbPassword
    aiur dotnet/seeddb $stargate_path "Stargate" $dbPassword
    aiur dotnet/seeddb $wrapgate_path "Wrapgate" $dbPassword
    aiur dotnet/seeddb $www_path "WWW" $dbPassword
    aiur dotnet/seeddb $wiki_path "Wiki" $dbPassword
    aiur dotnet/seeddb $status_path "Status" $dbPassword
    aiur dotnet/seeddb $account_path "Account" $dbPassword
    aiur dotnet/seeddb $colossus_path "Colossus" $dbPassword
    aiur dotnet/seeddb $wrap_path "Wrap" $dbPassword
    aiur dotnet/seeddb $ee_path "EE" $dbPassword
    rm $nexus_code -rf

    set_env $archon_path $1
    set_env $gateway_path $1
    set_env $developer_path $1
    set_env $observer_path $1
    set_env $probe_path $1
    set_env $stargate_path $1
    set_env $wrapgate_path $1
    set_env $www_path $1
    set_env $wiki_path $1
    set_env $status_path $1
    set_env $account_path $1
    set_env $colossus_path $1
    set_env $wrap_path $1
    set_env $ee_path $1

    aiur text/edit_json "ArchonEndpoint" "https://archon.$domain" $archon_path/appsettings.Production.json
    aiur text/edit_json "DeveloperEndpoint" "https://developer.$domain" $developer_path/appsettings.Production.json
    aiur text/edit_json "GatewayEndpoint" "https://gateway.$domain" $gateway_path/appsettings.Production.json
    aiur text/edit_json "ObserverEndpoint" "https://observer.$domain" $observer_path/appsettings.Production.json
    aiur text/edit_json "ProbeEndpoint" "https://probe.$domain" $probe_path/appsettings.Production.json
    aiur text/edit_json "OpenPattern" "https://probe.$domain/download/open/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "DownloadPattern" "https://probe.$domain/download/file/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "PlayerPattern" "https://probe.$domain/download/video/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "StargateEndpoint" "https://stargate.$domain" $stargate_path/appsettings.Production.json
    aiur text/edit_json "WrapgateEndpoint" "https://wrapgate.$domain" $wrapgate_path/appsettings.Production.json

    add_service "archon" $archon_path "Aiursoft.Archon" $1
    add_service "gateway" $gateway_path "Aiursoft.Gateway" $1
    add_service "developer" $developer_path "Aiursoft.Developer" $1
    add_service "observer" $observer_path "Aiursoft.Observer" $1
    add_service "probe" $probe_path "Aiursoft.Probe" $1
    add_service "stargate" $stargate_path "Aiursoft.Stargate" $1
    add_service "wrapgate" $wrapgate_path "Aiursoft.Wrapgate" $1
    add_service "www" $www_path "Aiursoft.WWW" $1
    add_service "wiki" $wiki_path "Aiursoft.Wiki" $1
    add_service "status" $status_path "Aiursoft.Status" $1
    add_service "account" $account_path "Aiursoft.Account" $1
    add_service "colossus" $colossus_path "Aiursoft.Colossus" $1
    add_service "wrap" $wrap_path "Aiursoft.Wrap" $1
    add_service "ee" $ee_path "Aiursoft.EE" $1

    echo 'Waitting for all services to start and init database...'
    sleep 10
    curl -sL https://github.com/AiursoftWeb/Nexus/raw/master/seed.sql --output - > ./temp.sql
    domainUpper=$(echo $domain | tr a-z A-Z)
    replace_in_file ./temp.sql "{{userId}}" $userId
    replace_in_file ./temp.sql "{{domain}}" $1
    replace_in_file ./temp.sql "{{domainUpper}}" $domainUpper
    replace_in_file ./temp.sql "{{developerAppId}}" $developerAppId
    replace_in_file ./temp.sql "{{developerAppSecret}}" $developerAppSecret
    aiur mssql/run_sql $dbPassword ./temp.sql

    aiur text/edit_json "DeveloperAppId" "$developerAppId" $developer_path/appsettings.Production.json
    aiur text/edit_json "DeveloperAppSecret" "$developerAppSecret" $developer_path/appsettings.Production.json

    # Finish the installation
    echo "The port 1433 is not opened. You can open your database to public via: sudo ufw allow 1433/tcp"
    echo ""
    echo "Successfully installed Nexus as a service in your machine!"
    echo "Database identity: $1:1433 with username: sa and password: $dbPassword."
    echo "Aiursoft identity: https://www.$1 with username: admin@$1 and password: admin123."
}

install_nexus "$@"
