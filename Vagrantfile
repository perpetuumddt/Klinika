# ============================================================
# Vagrant Configuration for BaGet NuGet Server
# ============================================================

Vagrant.configure("2") do |config|
  config.vm.synced_folder ".", "/vagrant"
  config.vm.boot_timeout = 600

  # ============================================================
  # SHARED PROVISIONING SCRIPT
  # ============================================================
  $shared_provision = <<-'SCRIPT'
    set -e  # Exit on error
    
    BAGET_PORT=$1
    OS_TYPE=$2
    
    # --------------------------------------------------------
    # 1. Install Dependencies
    # --------------------------------------------------------
    echo "=== Installing dependencies ==="
    if [ "$OS_TYPE" = "debian" ]; then
      apt-get update -y
      apt-get install -y wget unzip curl apt-transport-https
      
      # libssl1.1 for .NET 3.1 compatibility
      if [ ! -f /usr/lib/x86_64-linux-gnu/libssl.so.1.1 ]; then
        echo "Installing libssl1.1..."
        if grep -q "Ubuntu" /etc/os-release; then
          wget -q http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2_amd64.deb
        else
          wget -q http://security.debian.org/debian-security/pool/updates/main/o/openssl/libssl1.1_1.1.1w-0+deb11u1_amd64.deb
        fi
        dpkg -i libssl1.1_*.deb || true
        rm -f libssl1.1_*.deb
      fi
    else
      dnf install -y wget unzip curl
    fi

    # --------------------------------------------------------
    # 2. Install .NET SDK
    # --------------------------------------------------------
    echo "=== Installing .NET SDK ==="
    if [ ! -f /root/.dotnet/dotnet ]; then
      wget -q https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
      chmod +x dotnet-install.sh
      ./dotnet-install.sh --channel 3.1 --no-path  # For BaGet
      ./dotnet-install.sh --channel 9.0 --no-path  # For development
      rm dotnet-install.sh
    fi
    
    export PATH=/root/.dotnet:$PATH
    export DOTNET_ROOT=/root/.dotnet
    echo 'export PATH=/root/.dotnet:$PATH' >> /root/.bashrc
    echo 'export DOTNET_ROOT=/root/.dotnet' >> /root/.bashrc

    # --------------------------------------------------------
    # 3. Setup BaGet
    # --------------------------------------------------------
    echo "=== Setting up BaGet ==="
    mkdir -p /vagrant/packages ~/baget
    cd ~/baget
    
    if [ ! -f BaGet/BaGet.dll ]; then
      wget -q https://github.com/loic-sharma/BaGet/releases/download/v0.4.0-preview2/BaGet.zip
      unzip -qo BaGet.zip -d BaGet
      rm BaGet.zip
    fi
    
    cd BaGet
    cat > appsettings.Production.json <<'EOF'
{
  "ApiKey": "localkey",
  "Storage": {
    "Type": "FileSystem",
    "Path": "/vagrant/packages"
  },
  "Database": {
    "Type": "Sqlite",
    "ConnectionString": "Data Source=/vagrant/packages/baget.db"
  },
  "Mirror": { "Enabled": false }
}
EOF

    # --------------------------------------------------------
    # 4. Start BaGet
    # --------------------------------------------------------
    echo "=== Starting BaGet on port $BAGET_PORT ==="
    pkill -f "dotnet.*BaGet.dll" || true  # Kill existing instances
    nohup /root/.dotnet/dotnet BaGet.dll --urls http://0.0.0.0:$BAGET_PORT > baget.log 2>&1 &
    BAGET_PID=$!
    
    # Wait for BaGet to be ready
    for i in {1..30}; do
      if curl -sf http://127.0.0.1:$BAGET_PORT/v3/index.json >/dev/null; then
        echo "✓ BaGet is ready!"
        break
      fi
      
      if ! kill -0 $BAGET_PID 2>/dev/null; then
        echo "✗ ERROR: BaGet crashed! Logs:"
        cat baget.log
        exit 1
      fi
      
      [ $i -eq 30 ] && echo "✗ ERROR: Timeout waiting for BaGet" && cat baget.log && exit 1
      sleep 2
    done

    # --------------------------------------------------------
    # 5. Configure NuGet
    # --------------------------------------------------------
    echo "=== Configuring NuGet ==="
    mkdir -p ~/.nuget/NuGet
    cat > ~/.nuget/NuGet/NuGet.Config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="KlinikaRepo" value="http://127.0.0.1:$BAGET_PORT/v3/index.json" protocolVersion="3" allowInsecureConnections="true" />
  </packageSources>
</configuration>
EOF

    /root/.dotnet/dotnet nuget add source http://127.0.0.1:$BAGET_PORT/v3/index.json \
      --name KlinikaRepo --configfile ~/.nuget/NuGet/NuGet.Config 2>/dev/null || true

    # --------------------------------------------------------
    # 6. Create & Publish Test Package (Ubuntu only)
    # --------------------------------------------------------
    if [ "$OS_TYPE" = "ubuntu" ]; then
      echo "=== Creating test package ==="
      mkdir -p ~/testpackage && cd ~/testpackage
      
      if [ ! -d KlinikaApp ]; then
        /root/.dotnet/dotnet new classlib -n KlinikaApp -o KlinikaApp >/dev/null
        
        cat > KlinikaApp/KlinikaService.cs <<'CSHARP'
namespace KlinikaApp;
public class KlinikaService
{
    public string GetMessage() => "Hello from KlinikaApp v0.0.1!";
}
CSHARP

        cat > KlinikaApp/KlinikaApp.csproj <<'CSPROJ'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <PackageId>KlinikaApp</PackageId>
    <Version>0.0.1</Version>
    <Authors>Klinika Team</Authors>
    <Description>Test package for Klinika project</Description>
  </PropertyGroup>
</Project>
CSPROJ
      fi
      
      cd KlinikaApp
      /root/.dotnet/dotnet pack -c Release -o . >/dev/null
      /root/.dotnet/dotnet nuget push KlinikaApp.0.0.1.nupkg \
        --source http://127.0.0.1:$BAGET_PORT/v3/index.json \
        --api-key localkey --skip-duplicate >/dev/null 2>&1 || true
      echo "✓ Package published"
      
      # --------------------------------------------------------
      # 7. Test Package Installation
      # --------------------------------------------------------
      echo "=== Testing package installation ==="
      mkdir -p ~/test-app && cd ~/test-app
      /root/.dotnet/dotnet new console -n TestApp -o TestApp >/dev/null 2>&1 || true
      cd TestApp
      
      /root/.dotnet/dotnet add package KlinikaApp --version 0.0.1 \
        --source http://127.0.0.1:$BAGET_PORT/v3/index.json >/dev/null
      
      cat > Program.cs <<'PROGCS'
using KlinikaApp;
var service = new KlinikaService();
Console.WriteLine(service.GetMessage());
PROGCS

      echo "=== Running test app ==="
      /root/.dotnet/dotnet run
    fi
    
    echo "✓ Setup complete! BaGet running on port $BAGET_PORT"
    echo "  Web UI: http://localhost:$BAGET_PORT"
  SCRIPT

  # ============================================================
  # VIRTUAL MACHINES DEFINITION
  # ============================================================

  # Ubuntu 22.04 (Primary - creates and publishes package)
  config.vm.define "ubuntu_vm", primary: true do |vm|
    vm.vm.box = "ubuntu/jammy64"
    vm.vm.hostname = "baget-ubuntu"
    vm.vm.network "forwarded_port", guest: 5555, host: 5555
    vm.vm.provider "virtualbox" do |vb|
      vb.memory = 2048
      vb.cpus = 2
    end
    vm.vm.provision "shell", inline: $shared_provision, args: ["5555", "ubuntu"]
  end

  # Debian 12 (Tests package from Ubuntu)
  config.vm.define "debian", autostart: false do |vm|
    vm.vm.box = "debian/bookworm64"
    vm.vm.hostname = "baget-debian"
    vm.vm.network "forwarded_port", guest: 5556, host: 5556
    vm.vm.provision "shell", inline: $shared_provision, args: ["5556", "debian"]
  end

  # CentOS 8 (Tests package from Ubuntu)
  config.vm.define "centos", autostart: false do |vm|
    vm.vm.box = "centos/8"
    vm.vm.hostname = "baget-centos"
    vm.vm.network "forwarded_port", guest: 5557, host: 5557
    vm.vm.provision "shell", inline: $shared_provision, args: ["5557", "centos"]
  end
end