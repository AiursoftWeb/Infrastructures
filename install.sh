aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

nexus_code="./Nexus"
nexus_path="/opt/apps/Nexus"
dbPassword=$(uuidgen)

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

archon_code="$nexus_code/src/Web\ Services/Basic/Archon"
gateway_code="$nexus_code/src/Web\ Services/Basic/Gateway"
developer_code="$nexus_code/src/Web\ Services/Basic/Developer"
observer_code="$nexus_code/src/Web\ Services/Infrastructure/Observer"
probe_code="$nexus_code/src/Web\ Services/Infrastructure/Probe"
stargate_code="$nexus_code/src/Web\ Services/Infrastructure/Stargate"
wrapgate_code="$nexus_code/src/Web\ Services/Infrastructure/Wrapgate"
www_code="$nexus_code/src/Web\ Services/Business/WWW"
wiki_code="$nexus_code/src/Web\ Services/Business/Wiki"
status_code="$nexus_code/src/Web\ Services/Business/Status"
account_code="$nexus_code/src/Web\ Services/Business/Account"
colossus_code="$nexus_code/src/Web\ Services/Business/Colossus"
wrap_code="$nexus_code/src/Web\ Services/Business/Wrap"
ee_code="$nexus_code/src/Web\ Services/Business/EE"

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
    # dotnet publish -c Release -o $kahla_path ./Kahla/Kahla.Server/Kahla.Server.csproj && rm ./Kahla -rf
    # cat $kahla_path/appsettings.json > $kahla_path/appsettings.Production.json

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
