aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

nexus_code="./Nexus"
nexus_path="/opt/apps/Nexus"
dbPassword=$(uuidgen)

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

build_to()
{
    code_path="$1"
    dist_path="$2"
    project_name="$3"
    dotnet publish -c Release -o $dist_path $code_path/$project_name.csproj
    cat $dist_path/appsettings.json > $dist_path/appsettings.Production.json
}

set_db()
{
    dist_path="$1"
    db_name="$2"
    domain="$3"
    connectionString="Server=tcp:127.0.0.1,1433;Database=$db_name;uid=sa;Password=$dbPassword;MultipleActiveResultSets=True;"
    aiur text/edit_json "ConnectionStrings.DatabaseConnection" "$connectionString" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ArchonConnection" "https://archon.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.GatewayConnection" "https://gateway.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.StargateConnection" "https://stargate.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ObserverConnection" "https://observer.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.ProbeConnection" "https://probe.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "ConnectionStrings.WrapgateConnection" "https://wrapgate.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.AccountPath" "https://account.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.UIPath" "https://ui.$domain" $dist_path/appsettings.Production.json
    aiur text/edit_json "Dependencies.ColossusPath" "https://colossus.$domain" $dist_path/appsettings.Production.json
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
    build_to $archon_code $archon_path "Aiursoft.Archon"
    build_to $gateway_code $gateway_path "Aiursoft.Gateway"
    build_to $developer_code $developer_path "Aiursoft.Developer"
    build_to $observer_code $observer_path "Aiursoft.Observer"
    build_to $probe_code $probe_path "Aiursoft.Probe"
    build_to $stargate_code $stargate_path "Aiursoft.Stargate"
    build_to $wrapgate_code $wrapgate_path "Aiursoft.Wrapgate"
    build_to $www_code $www_path "Aiursoft.WWW"
    build_to $wiki_code $wiki_path "Aiursoft.Wiki"
    build_to $status_code $status_path "Aiursoft.Status"
    build_to $account_code $account_path "Aiursoft.Account"
    build_to $colossus_code $colossus_path "Aiursoft.Colossus"
    build_to $wrap_code $wrap_path "Aiursoft.Wrap"
    build_to $ee_code $ee_path "Aiursoft.EE"
    rm $nexus_code -rf

    set_db $gateway_path "Gateway" $1
    set_db $developer_path "Developer" $1
    set_db $observer_path "Observer" $1
    set_db $probe_path "Probe" $1
    set_db $stargate_path "Stargate" $1
    set_db $wrapgate_path "Wrapgate" $1
    set_db $www_path "WWW" $1
    set_db $wiki_path "Wiki" $1
    set_db $status_path "Status" $1
    set_db $account_path "Account" $1
    set_db $colossus_path "Colossus" $1
    set_db $wrap_path "Wrap" $1
    set_db $ee_path "EE" $1

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

    # aiur text/edit_json "ConnectionStrings.DatabaseConnection" "$connectionString" $kahla_path/appsettings.Production.json
    # aiur text/edit_json "KahlaAppId" "$2" $kahla_path/appsettings.Production.json
    # aiur text/edit_json "KahlaAppSecret" "$3" $kahla_path/appsettings.Production.json
    # aiur services/register_aspnet_service "kahla" $port $kahla_path "Kahla.Server"
    # aiur caddy/add_proxy $1 $port

    # Finish the installation
    echo "Successfully installed Nexus as a service in your machine! Please open https://www.$1 to try it now!"
    echo "The port 1433 is not opened. You can open your database to public via: sudo ufw allow 1433/tcp"
    echo "Database identity: $1:1433 with username: sa and password: $dbPassword"
}

install_nexus "$@"
