aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

update()
{
    instance_name="$1"
    
    # Upgrade AiurUI
    curl -sL https://github.com/AiursoftWeb/AiurUI/raw/master/upgrade.sh | sudo bash

    nexus_code="./Nexus"
    nexus_path="/opt/apps/Nexus"

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

    aiur git/clone_to AiursoftWeb/Nexus $nexus_code
    sed -i -e "s/\"Aiursoft\"/\"$instance_name\"/g" $nexus_code/src/SDK/SDK/Values.cs

    systemctl stop "archon.service"
    aiur dotnet/publish $archon_path $archon_code/"Aiursoft.Archon.csproj"
    systemctl start "archon.service"

    systemctl stop "stargate.service"
    aiur dotnet/publish $stargate_path $stargate_code/"Aiursoft.Stargate.csproj"
    systemctl start "stargate.service"

    systemctl stop "probe.service"
    aiur dotnet/publish $probe_path $probe_code/"Aiursoft.Probe.csproj"
    systemctl start "probe.service"

    systemctl stop "gateway.service"
    aiur dotnet/publish $gateway_path $gateway_code/"Aiursoft.Gateway.csproj"
    systemctl start "gateway.service"

    systemctl stop "wrapgate.service"
    aiur dotnet/publish $wrapgate_path $wrapgate_code/"Aiursoft.Wrapgate.csproj"
    systemctl start "wrapgate.service"

    systemctl stop "observer.service"
    aiur dotnet/publish $observer_path $observer_code/"Aiursoft.Observer.csproj"
    systemctl start "observer.service"

    systemctl stop "developer.service"
    aiur dotnet/publish $developer_path $developer_code/"Aiursoft.Developer.csproj"
    systemctl start "developer.service"

    systemctl stop "www.service"
    aiur dotnet/publish $www_path $www_code/"Aiursoft.WWW.csproj"
    systemctl start "www.service"

    systemctl stop "wiki.service"
    aiur dotnet/publish $wiki_path $wiki_code/"Aiursoft.Wiki.csproj"
    systemctl start "wiki.service"

    systemctl stop "account.service"
    aiur dotnet/publish $account_path $account_code/"Aiursoft.Account.csproj"
    systemctl start "account.service"

    systemctl stop "colossus.service"
    aiur dotnet/publish $colossus_path $colossus_code/"Aiursoft.Colossus.csproj"
    systemctl start "colossus.service"

    systemctl stop "wrap.service"
    aiur dotnet/publish $wrap_path $wrap_code/"Aiursoft.Wrap.csproj"
    systemctl start "wrap.service"

    systemctl stop "ee.service"
    aiur dotnet/publish $ee_path $ee_code/"Aiursoft.EE.csproj"
    systemctl start "ee.service"

    systemctl stop "status.service"
    aiur dotnet/publish $status_path $status_code/"Aiursoft.Status.csproj"
    systemctl start "status.service"

    rm $nexus_code -rf
}

update "$@"
