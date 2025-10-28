aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.com/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }

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
    curl -sL https://gitlab.aiursoft.com/aiursoft/aiurui/-/raw/master/upgrade.sh | sudo bash

    infs_code="./Infrastructures"
    infs_path="/opt/apps/Infrastructures"

    directory_code="$infs_code/src/WebServices/Basic/Directory"
    developer_code="$infs_code/src/WebServices/Basic/Developer"
    observer_code="$infs_code/src/WebServices/Infrastructure/Observer"
    probe_code="$infs_code/src/WebServices/Infrastructure/Probe"
    stargate_code="$infs_code/src/WebServices/Infrastructure/Stargate"
    warpgate_code="$infs_code/src/WebServices/Infrastructure/Warpgate"
    www_code="$infs_code/src/WebServices/Business/WWW"
    wiki_code="$infs_code/src/WebServices/Business/Wiki"
    status_code="$infs_code/src/WebServices/Business/Status"
    account_code="$infs_code/src/WebServices/Business/Account"
    ee_code="$infs_code/src/WebServices/Business/EE"

    directory_path="$infs_path/Directory"
    developer_path="$infs_path/Developer"
    observer_path="$infs_path/Observer"
    probe_path="$infs_path/Probe"
    stargate_path="$infs_path/Stargate"
    warpgate_path="$infs_path/Warpgate"
    www_path="$infs_path/WWW"
    wiki_path="$infs_path/Wiki"
    status_path="$infs_path/Status"
    account_path="$infs_path/Account"
    ee_path="$infs_path/EE"

    aiur git/clone_to https://gitlab.aiursoft.com/aiursoft/Infrastructures $infs_code $branch_name
    sed -i -e "s/\"Aiursoft\"/\"$instance_name\"/g" $infs_code/src/SDK/SDK/Values.cs

    systemctl stop "stargate.service"
    aiur dotnet/publish $stargate_path $stargate_code/"Aiursoft.Stargate.csproj"

    systemctl stop "probe.service"
    aiur dotnet/publish $probe_path $probe_code/"Aiursoft.Probe.csproj"

    systemctl stop "directory.service"
    aiur dotnet/publish $directory_path $directory_code/"Aiursoft.Directory.csproj"

    systemctl stop "warpgate.service"
    aiur dotnet/publish $warpgate_path $warpgate_code/"Aiursoft.Warpgate.csproj"

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

    systemctl stop "ee.service"
    aiur dotnet/publish $ee_path $ee_code/"Aiursoft.EE.csproj"

    systemctl stop "status.service"
    aiur dotnet/publish $status_path $status_code/"Aiursoft.Status.csproj"
    
    echo "Starting infrastructurs..."
    systemctl restart "probe.service"
    systemctl restart "stargate.service"
    systemctl restart "warpgate.service"
    systemctl restart "observer.service"
    sleep 30
    
    echo "Starting basic services..."
    systemctl restart "directory.service"
    systemctl restart "developer.service"
    sleep 30
    
    echo "Starting business services..."
    systemctl restart "www.service"
    systemctl restart "account.service"
    systemctl restart "ee.service"
    systemctl restart "wiki.service"
    sleep 30
    
    echo "Starting monitoring services..."
    systemctl restart "status.service"
    systemctl restart "ee.service"

    rm $infs_code -rf
}

update "$@"
