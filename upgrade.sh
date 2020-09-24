aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

update()
{
    instance_name=$1;

    if [[ $instance_name == "" ]];
    then
        instance_name="Aiursoft"
        echo "Using instance name: $instance_name"
    else
        echo "Instance name is: $instance_name"
    fi
    
    branch_name=$2;

    if [[ $branch_name == "" ]];
    then
        branch_name="master"
        echo "Using branch name: $branch_name"
    else
        echo "branch name is: $branch_name"
    fi
    
    # Upgrade AiurUI
    #curl -sL https://github.com/AiursoftWeb/AiurUI/raw/master/upgrade.sh | sudo bash

    infs_code="./Infrastructures"
    infs_path="/opt/apps/Infrastructures"

    archon_code="$infs_code/src/WebServices/Basic/Archon"
    gateway_code="$infs_code/src/WebServices/Basic/Gateway"
    developer_code="$infs_code/src/WebServices/Basic/Developer"
    observer_code="$infs_code/src/WebServices/Infrastructure/Observer"
    probe_code="$infs_code/src/WebServices/Infrastructure/Probe"
    stargate_code="$infs_code/src/WebServices/Infrastructure/Stargate"
    wrapgate_code="$infs_code/src/WebServices/Infrastructure/Wrapgate"
    www_code="$infs_code/src/WebServices/Business/WWW"
    wiki_code="$infs_code/src/WebServices/Business/Wiki"
    status_code="$infs_code/src/WebServices/Business/Status"
    account_code="$infs_code/src/WebServices/Business/Account"
    colossus_code="$infs_code/src/WebServices/Business/Colossus"
    wrap_code="$infs_code/src/WebServices/Business/Wrap"
    ee_code="$infs_code/src/WebServices/Business/EE"

    archon_path="$infs_path/Archon"
    gateway_path="$infs_path/Gateway"
    developer_path="$infs_path/Developer"
    observer_path="$infs_path/Observer"
    probe_path="$infs_path/Probe"
    stargate_path="$infs_path/Stargate"
    wrapgate_path="$infs_path/Wrapgate"
    www_path="$infs_path/WWW"
    wiki_path="$infs_path/Wiki"
    status_path="$infs_path/Status"
    account_path="$infs_path/Account"
    colossus_path="$infs_path/Colossus"
    wrap_path="$infs_path/Wrap"
    ee_path="$infs_path/EE"

    aiur git/clone_to AiursoftWeb/Infrastructures $infs_code $branch_name
    sed -i -e "s/\"Aiursoft\"/\"$instance_name\"/g" $infs_code/src/SDK/SDK/Values.cs

    systemctl stop "archon.service"
    aiur dotnet/publish $archon_path $archon_code/"Aiursoft.Archon.csproj"

    systemctl stop "stargate.service"
    aiur dotnet/publish $stargate_path $stargate_code/"Aiursoft.Stargate.csproj"

    systemctl stop "probe.service"
    aiur dotnet/publish $probe_path $probe_code/"Aiursoft.Probe.csproj"

    systemctl stop "gateway.service"
    aiur dotnet/publish $gateway_path $gateway_code/"Aiursoft.Gateway.csproj"

    systemctl stop "wrapgate.service"
    aiur dotnet/publish $wrapgate_path $wrapgate_code/"Aiursoft.Wrapgate.csproj"

    systemctl stop "observer.service"
    aiur dotnet/publish $observer_path $observer_code/"Aiursoft.Observer.csproj"

    systemctl stop "developer.service"
    aiur dotnet/publish $developer_path $developer_code/"Aiursoft.Developer.csproj"

    systemctl stop "www.service"
    aiur dotnet/publish $www_path $www_code/"Aiursoft.WWW.csproj"

    systemctl stop "wiki.service"
    aiur dotnet/publish $wiki_path $wiki_code/"Aiursoft.Wiki.csproj"

    systemctl stop "account.service"
    aiur dotnet/publish $account_path $account_code/"Aiursoft.Account.csproj"

    systemctl stop "colossus.service"
    aiur dotnet/publish $colossus_path $colossus_code/"Aiursoft.Colossus.csproj"

    systemctl stop "wrap.service"
    aiur dotnet/publish $wrap_path $wrap_code/"Aiursoft.Wrap.csproj"

    systemctl stop "ee.service"
    aiur dotnet/publish $ee_path $ee_code/"Aiursoft.EE.csproj"

    systemctl stop "status.service"
    aiur dotnet/publish $status_path $status_code/"Aiursoft.Status.csproj"

    systemctl restart mssql.service
    systemctl restart "archon.service"
    sleep 30
    systemctl restart "probe.service"
    systemctl restart "stargate.service"
    systemctl restart "wrapgate.service"
    systemctl restart "observer.service"
    sleep 30
    systemctl restart "gateway.service"
    systemctl restart "developer.service"
    sleep 30
    systemctl restart "colossus.service"
    systemctl restart "www.service"
    systemctl restart "account.service"
    systemctl restart "wrap.service"
    systemctl restart "ee.service"
    systemctl restart "wiki.service"
    sleep 30
    systemctl restart "status.service"
    systemctl restart "ee.service"

    rm $infs_code -rf
}

update "$@"
