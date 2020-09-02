aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

nexus_code="./Nexus"
nexus_path="/opt/apps/Nexus"
dbPassword=$(uuidgen)
userId=$(uuidgen)

developerAppId=$(uuidgen)
developerAppSecret=$(uuidgen)
gatewayAppId=$(uuidgen)
gatewayAppSecret=$(uuidgen)
stargateAppId=$(uuidgen)
stargateAppSecret=$(uuidgen)
observerAppId=$(uuidgen)
observerAppSecret=$(uuidgen)
probeAppId=$(uuidgen)
probeAppSecret=$(uuidgen)
wrapgateAppId=$(uuidgen)
wrapgateAppSecret=$(uuidgen)
wwwAppId=$(uuidgen)
wwwAppSecret=$(uuidgen)
wikiAppId=$(uuidgen)
wikiAppSecret=$(uuidgen)
accountAppId=$(uuidgen)
accountAppSecret=$(uuidgen)
statusAppId=$(uuidgen)
statusAppSecret=$(uuidgen)
colossusAppId=$(uuidgen)
colossusAppSecret=$(uuidgen)
wrapAppId=$(uuidgen)
wrapAppSecret=$(uuidgen)
eeAppId=$(uuidgen)
eeAppSecret=$(uuidgen)

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
    aiur text/edit_json "Dependencies.DeveloperPath" "https://developer.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.GatewayPath" "https://gateway.$domain" $dist_path/appsettings.Production.json
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
    curl -s https://$name.$domain/ --output - > /dev/null # Init a request to let caddy config its cert.
    sleep 1
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

    instance_name=$2;

    if [[ $instance_name == "" ]];
    then
        instance_name="Aiursoft"
        echo "Using instance name: $instance_name"
    else
        echo "Instance name is: $instance_name"
    fi

    curl -sL https://github.com/AiursoftWeb/AiurUI/raw/master/install.sh | bash -s ui.$1

    aiur system/set_aspnet_prod
    aiur install/dotnet
    aiur install/jq
    aiur install/sql_server
    aiur mssql/config_password $dbPassword

    aiur git/clone_to AiursoftWeb/Nexus $nexus_code
    sed -i -e "s/\"Aiursoft\"/\"$instance_name\"/g" $nexus_code/src/SDK/SDK/Values.cs
    dotnet restore $nexus_code/Nexus.sln
    cp $archon_code/appsettings.json $archon_code/appsettings.Production.json
    aiur dotnet/seeddb $gateway_code "Gateway" $dbPassword
    aiur dotnet/seeddb $developer_code "Developer" $dbPassword
    aiur dotnet/seeddb $observer_code "Observer" $dbPassword
    aiur dotnet/seeddb $probe_code "Probe" $dbPassword
    aiur dotnet/seeddb $stargate_code "Stargate" $dbPassword
    aiur dotnet/seeddb $wrapgate_code "Wrapgate" $dbPassword
    aiur dotnet/seeddb $www_code "WWW" $dbPassword
    aiur dotnet/seeddb $wiki_code "Wiki" $dbPassword
    aiur dotnet/seeddb $status_code "Status" $dbPassword
    aiur dotnet/seeddb $account_code "Account" $dbPassword
    aiur dotnet/seeddb $colossus_code "Colossus" $dbPassword
    aiur dotnet/seeddb $wrap_code "Wrap" $dbPassword
    aiur dotnet/seeddb $ee_code "EE" $dbPassword

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

    aiur text/edit_json "ArchonEndpoint" "https://archon.$1" $archon_path/appsettings.Production.json
    aiur text/edit_json "DeveloperEndpoint" "https://developer.$1" $developer_path/appsettings.Production.json
    aiur text/edit_json "GatewayEndpoint" "https://gateway.$1" $gateway_path/appsettings.Production.json
    aiur text/edit_json "ObserverEndpoint" "https://observer.$1" $observer_path/appsettings.Production.json
    aiur text/edit_json "ProbeEndpoint" "https://probe.$1" $probe_path/appsettings.Production.json
    aiur text/edit_json "OpenPattern" "https://probe.$1/download/open/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "DownloadPattern" "https://probe.$1/download/file/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "PlayerPattern" "https://probe.$1/download/video/{0}" $probe_path/appsettings.Production.json
    aiur text/edit_json "StargateEndpoint" "https://stargate.$1" $stargate_path/appsettings.Production.json
    aiur text/edit_json "WrapgateEndpoint" "https://wrapgate.$1" $wrapgate_path/appsettings.Production.json
    aiur text/edit_json "WrapPattern" "https://wrapgate.$1/wrap/{wrap}" $wrapgate_path/appsettings.Production.json
    aiur text/edit_json "StoragePath" "/opt/apps/" $probe_path/appsettings.Production.json
    aiur text/edit_json "TempFileStoragePath" "/tmp/probe" $probe_path/appsettings.Production.json
    aiur text/edit_json "DeveloperAppId" "$developerAppId" $developer_path/appsettings.Production.json
    aiur text/edit_json "DeveloperAppSecret" "$developerAppSecret" $developer_path/appsettings.Production.json
    aiur text/edit_json "GatewayAppId" "$gatewayAppId" $gateway_path/appsettings.Production.json
    aiur text/edit_json "GatewayAppSecret" "$gatewayAppSecret" $gateway_path/appsettings.Production.json
    aiur text/edit_json "TestAppId" "$stargateAppId" $stargate_path/appsettings.Production.json
    aiur text/edit_json "TestAppSecret" "$stargateAppSecret" $stargate_path/appsettings.Production.json
    aiur text/edit_json "ObserverAppId" "$observerAppId" $observer_path/appsettings.Production.json
    aiur text/edit_json "ObserverAppSecret" "$observerAppSecret" $observer_path/appsettings.Production.json
    aiur text/edit_json "ProbeAppId" "$probeAppId" $probe_path/appsettings.Production.json
    aiur text/edit_json "ProbeAppSecret" "$probeAppSecret" $probe_path/appsettings.Production.json
    aiur text/edit_json "WrapgateAppId" "$wrapgateAppId" $wrapgate_path/appsettings.Production.json
    aiur text/edit_json "WrapgateAppSecret" "$wrapgateAppSecret" $wrapgate_path/appsettings.Production.json
    aiur text/edit_json "WWWAppId" "$wwwAppId" $www_path/appsettings.Production.json
    aiur text/edit_json "WWWAppSecret" "$wwwAppSecret" $www_path/appsettings.Production.json
    aiur text/edit_json "WikiAppId" "$wikiAppId" $wiki_path/appsettings.Production.json
    aiur text/edit_json "WikiAppSecret" "$wikiAppSecret" $wiki_path/appsettings.Production.json
    aiur text/edit_json "AccountAppId" "$accountAppId" $account_path/appsettings.Production.json
    aiur text/edit_json "AccountAppSecret" "$accountAppSecret" $account_path/appsettings.Production.json
    aiur text/edit_json "StatusAppId" "$statusAppId" $status_path/appsettings.Production.json
    aiur text/edit_json "StatusAppSecret" "$statusAppSecret" $status_path/appsettings.Production.json
    aiur text/edit_json "ColossusAppId" "$colossusAppId" $colossus_path/appsettings.Production.json
    aiur text/edit_json "ColossusAppSecret" "$colossusAppSecret" $colossus_path/appsettings.Production.json
    aiur text/edit_json "WrapAppId" "$wrapAppId" $wrap_path/appsettings.Production.json
    aiur text/edit_json "WrapAppSecret" "$wrapAppSecret" $wrap_path/appsettings.Production.json
    aiur text/edit_json "EEAppId" "$eeAppId" $ee_path/appsettings.Production.json
    aiur text/edit_json "EEAppSecret" "$eeAppSecret" $ee_path/appsettings.Production.json

    curl -sL https://github.com/AiursoftWeb/Nexus/raw/master/seed.sql --output - > ./temp.sql
    domainUpper=$(echo $domain | tr a-z A-Z)
    replace_in_file ./temp.sql "{{Instance}}" $instance_name
    replace_in_file ./temp.sql "{{userId}}" $userId
    replace_in_file ./temp.sql "{{domain}}" $1
    replace_in_file ./temp.sql "{{domainUpper}}" $domainUpper
    replace_in_file ./temp.sql "{{developerAppId}}" $developerAppId
    replace_in_file ./temp.sql "{{developerAppSecret}}" $developerAppSecret
    replace_in_file ./temp.sql "{{gatewayAppId}}" $gatewayAppId
    replace_in_file ./temp.sql "{{gatewayAppSecret}}" $gatewayAppSecret
    replace_in_file ./temp.sql "{{stargateAppId}}" $stargateAppId
    replace_in_file ./temp.sql "{{stargateAppSecret}}" $stargateAppSecret
    replace_in_file ./temp.sql "{{observerAppId}}" $observerAppId
    replace_in_file ./temp.sql "{{observerAppSecret}}" $observerAppSecret
    replace_in_file ./temp.sql "{{probeAppId}}" $probeAppId
    replace_in_file ./temp.sql "{{probeAppSecret}}" $probeAppSecret
    replace_in_file ./temp.sql "{{wrapgateAppId}}" $wrapgateAppId
    replace_in_file ./temp.sql "{{wrapgateAppSecret}}" $wrapgateAppSecret
    replace_in_file ./temp.sql "{{wikiAppId}}" $wikiAppId
    replace_in_file ./temp.sql "{{wikiAppSecret}}" $wikiAppSecret
    replace_in_file ./temp.sql "{{wwwAppId}}" $wwwAppId
    replace_in_file ./temp.sql "{{wwwAppSecret}}" $wwwAppSecret
    replace_in_file ./temp.sql "{{accountAppId}}" $accountAppId
    replace_in_file ./temp.sql "{{accountAppSecret}}" $accountAppSecret
    replace_in_file ./temp.sql "{{statusAppId}}" $statusAppId
    replace_in_file ./temp.sql "{{statusAppSecret}}" $statusAppSecret
    replace_in_file ./temp.sql "{{colossusAppId}}" $colossusAppId
    replace_in_file ./temp.sql "{{colossusAppSecret}}" $colossusAppSecret
    replace_in_file ./temp.sql "{{wrapAppId}}" $wrapAppId
    replace_in_file ./temp.sql "{{wrapAppSecret}}" $wrapAppSecret
    replace_in_file ./temp.sql "{{eeAppId}}" $eeAppId
    replace_in_file ./temp.sql "{{eeAppSecret}}" $eeAppSecret
    aiur mssql/run_sql $dbPassword ./temp.sql

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

    echo 'Waitting for all services to start and config certificate...'
    
    sleep 30

    # Finish the installation
    echo "The port 1433 is not opened. You can open your database to public via: sudo ufw allow 1433/tcp"
    echo ""
    echo "Successfully installed Nexus as a service in your machine!"
    echo "Database identity: $1:1433 with username: sa and password: $dbPassword."
    echo "Aiursoft identity: https://www.$1 with username: admin@$1 and password: admin123."
}

install_nexus "$@"
