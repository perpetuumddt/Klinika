Vagrant.configure("2") do |config|
  config.vm.synced_folder ".", "/vagrant"
  config.vm.boot_timeout = 600

  config.vm.define "ubuntu_vm" do |ubuntu|
    ubuntu.vm.box = "ubuntu/jammy64"
    ubuntu.vm.hostname = "baget-ubuntu"
    ubuntu.vm.network "forwarded_port", guest: 5555, host: 5555

    ubuntu.vm.provider "virtualbox" do |vb|
      vb.memory = 2048
      vb.cpus = 2
    end

    ubuntu.vm.provision "shell", inline: <<-SHELL
      sudo apt-get update -y
      sudo apt-get install -y wget unzip curl apt-transport-https
      
      # libssl1.1 for .NET 3.1 (Ubuntu 22.04)
      echo "Installing libssl1.1 for .NET 3.1 compatibility..."
      wget http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2_amd64.deb
      sudo dpkg -i libssl1.1_1.1.1f-1ubuntu2_amd64.deb
      rm libssl1.1_1.1.1f-1ubuntu2_amd64.deb

      echo "Installing .NET SDK..."
      wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
      chmod +x dotnet-install.sh
      
      # .NET 3.1 for BaGet
      ./dotnet-install.sh --channel 3.1
      # .NET 9.0 for client
      ./dotnet-install.sh --channel 9.0
      
      # Global enviroment variables
      echo 'export PATH=$PATH:/root/.dotnet' >> /root/.bashrc
      echo 'export DOTNET_ROOT=/root/.dotnet' >> /root/.bashrc
      export PATH=$PATH:/root/.dotnet
      export DOTNET_ROOT=/root/.dotnet

      echo "Setting up BaGet..."
      # Creating package directory
      mkdir -p /vagrant/packages
      
      mkdir -p ~/baget
      cd ~/baget
      
      # Using .NET compatible BaGet version
      wget https://github.com/loic-sharma/BaGet/releases/download/v0.4.0-preview2/BaGet.zip -O BaGet.zip
      unzip -o BaGet.zip -d BaGet
      cd BaGet

      cat <<'EOF' > appsettings.Production.json
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

      nohup /root/.dotnet/dotnet BaGet.dll --urls http://0.0.0.0:5555 > baget.log 2>&1 &
      BAGET_PID=$!
      echo "BaGet started with PID: $BAGET_PID"
      echo "Waiting for BaGet to start..."
      
      for i in {1..30}; do
        if curl -s http://127.0.0.1:5555/v3/index.json > /dev/null 2>&1; then
          echo "BaGet is ready!"
          break
        fi
        echo "Attempt $i/30: BaGet not ready yet..."
        
        # Checking status
        if ! kill -0 $BAGET_PID 2>/dev/null; then
          echo "ERROR: BaGet process died! Check logs:"
          cat baget.log
          exit 1
        fi
        
        sleep 2
      done
      
      # Checking status
      if ! curl -s http://127.0.0.1:5555/v3/index.json > /dev/null 2>&1; then
        echo "ERROR: BaGet failed to start after 60 seconds. Logs:"
        cat baget.log
        exit 1
      fi
      
      sleep 5

      echo "=== Configuring NuGet ==="
      # Creating NuGet.Config
      mkdir -p ~/.nuget/NuGet
      cat > ~/.nuget/NuGet/NuGet.Config <<'EOL'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="KlinikaRepo" value="http://127.0.0.1:5555/v3/index.json" protocolVersion="3" allowInsecureConnections="true" />
  </packageSources>
</configuration>
EOL

      # adding source through CLI (для .NET 9.0)
      /root/.dotnet/dotnet nuget add source http://127.0.0.1:5555/v3/index.json \
        --name KlinikaRepo \
        --configfile ~/.nuget/NuGet/NuGet.Config || true

      echo "NuGet sources:"
      /root/.dotnet/dotnet nuget list source

      echo "=== Creating and publishing test package ==="
      mkdir -p ~/testpackage
      cd ~/testpackage
      /root/.dotnet/dotnet new classlib -n KlinikaApp
      cd KlinikaApp
      
      # Welcome script
      cat > Class1.cs <<'CSHARP'
namespace KlinikaApp;

public class KlinikaService
{
    public string GetMessage()
    {
        return "Hello from KlinikaApp!";
    }
}
CSHARP

      # Updating .csproj for package creating
      cat > KlinikaApp.csproj <<'CSPROJ'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PackageId>KlinikaApp</PackageId>
    <Version>0.0.1</Version>
    <Authors>Klinika Team</Authors>
    <Description>Test package for Klinika project</Description>
  </PropertyGroup>
</Project>
CSPROJ

      # Building package
      /root/.dotnet/dotnet pack -c Release -o .
      
      # push to BaGet
      /root/.dotnet/dotnet nuget push KlinikaApp.0.0.1.nupkg --source http://127.0.0.1:5555/v3/index.json --api-key localkey
      
      echo "Package published successfully!"
      sleep 2

      echo "=== Creating test project ==="
      mkdir -p ~/klinikaapp
      cd ~/klinikaapp
      /root/.dotnet/dotnet new console -n TestApp
      cd TestApp

      echo "=== Restoring ==="
      /root/.dotnet/dotnet restore

      echo "=== Adding KlinikaApp package ==="
      # Using --prerelease if package is pre-release, remove --version if no package
      /root/.dotnet/dotnet add package KlinikaApp --version 0.0.1 --source http://127.0.0.1:5555/v3/index.json || echo "Package not found, you may need to push it first"
      
      echo "=== Final restore ==="
      /root/.dotnet/dotnet restore
      
      # Updating Program.cs to use package
      cat > Program.cs <<'PROGCS'
using KlinikaApp;

var service = new KlinikaService();
Console.WriteLine(service.GetMessage());
PROGCS

      echo "=== Building and running test app ==="
      /root/.dotnet/dotnet build
      /root/.dotnet/dotnet run
      
      echo "=== Setup complete! ==="
    SHELL
  end

  # === Debian 12 ===
  config.vm.define "debian" do |debian|
    debian.vm.box = "debian/bookworm64"
    debian.vm.hostname = "debian-baget"
    debian.vm.network "forwarded_port", guest: 5556, host: 5556

    debian.vm.provision "shell", inline: <<-SHELL
      echo "=== Debian: Installing dependencies ==="
      sudo apt-get update -y
      sudo apt-get install -y wget unzip curl apt-transport-https
      
      echo "Installing libssl1.1 for .NET 3.1 compatibility..."
      wget http://security.debian.org/debian-security/pool/updates/main/o/openssl/libssl1.1_1.1.1w-0+deb11u1_amd64.deb
      sudo dpkg -i libssl1.1_1.1.1w-0+deb11u1_amd64.deb || true
      rm libssl1.1_1.1.1w-0+deb11u1_amd64.deb

      wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
      chmod +x dotnet-install.sh
      ./dotnet-install.sh --channel 3.1
      ./dotnet-install.sh --channel 9.0
      
      echo 'export PATH=$PATH:/root/.dotnet' >> /root/.bashrc
      echo 'export DOTNET_ROOT=/root/.dotnet' >> /root/.bashrc
      export PATH=$PATH:/root/.dotnet
      export DOTNET_ROOT=/root/.dotnet

      mkdir -p /vagrant/packages
      mkdir -p ~/baget
      cd ~/baget
      wget https://github.com/loic-sharma/BaGet/releases/download/v0.4.0-preview2/BaGet.zip -O BaGet.zip
      unzip -o BaGet.zip -d BaGet
      cd BaGet

      cat <<'EOF' > appsettings.Production.json
{
  "ApiKey": "localkey",
  "Storage": { "Type": "FileSystem", "Path": "/vagrant/packages" },
  "Database": { "Type": "Sqlite", "ConnectionString": "Data Source=/vagrant/packages/baget.db" },
  "Mirror": { "Enabled": false }
}
EOF

      nohup /root/.dotnet/dotnet BaGet.dll --urls http://0.0.0.0:5556 > baget.log 2>&1 &
      BAGET_PID=$!
      echo "BaGet started with PID: $BAGET_PID"
      
      for i in {1..30}; do
        if curl -s http://127.0.0.1:5556/v3/index.json > /dev/null 2>&1; then
          echo "BaGet is ready!"
          break
        fi
        echo "Attempt $i/30: BaGet not ready yet..."
        if ! kill -0 $BAGET_PID 2>/dev/null; then
          echo "ERROR: BaGet process died! Check logs:"
          cat baget.log
          exit 1
        fi
        sleep 2
      done
      
      if ! curl -s http://127.0.0.1:5556/v3/index.json > /dev/null 2>&1; then
        echo "ERROR: BaGet failed to start. Logs:"
        cat baget.log
        exit 1
      fi
      sleep 5

      mkdir -p ~/.nuget/NuGet
      cat > ~/.nuget/NuGet/NuGet.Config <<'EOL'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="KlinikaRepo" value="http://127.0.0.1:5556/v3/index.json" protocolVersion="3" allowInsecureConnections="true" />
  </packageSources>
</configuration>
EOL

      /root/.dotnet/dotnet nuget add source http://127.0.0.1:5556/v3/index.json \
        --name KlinikaRepo \
        --configfile ~/.nuget/NuGet/NuGet.Config || true

      mkdir -p ~/klinikaapp
      cd ~/klinikaapp
      /root/.dotnet/dotnet new console -n TestApp
      cd TestApp
      /root/.dotnet/dotnet add package KlinikaApp --version 0.0.1 --source http://127.0.0.1:5556/v3/index.json || echo "Package not found"
      /root/.dotnet/dotnet restore
    SHELL
  end

  # === CentOS 8 ===
  config.vm.define "centos" do |centos|
    centos.vm.box = "centos/8"
    centos.vm.hostname = "centos-baget"
    centos.vm.network "forwarded_port", guest: 5557, host: 5557

    centos.vm.provision "shell", inline: <<-SHELL
      echo "=== CentOS: Installing dependencies ==="
      sudo dnf install -y wget unzip curl

      wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
      chmod +x dotnet-install.sh
      ./dotnet-install.sh --channel 3.1
      ./dotnet-install.sh --channel 9.0
      
      echo 'export PATH=$PATH:/root/.dotnet' >> /root/.bashrc
      echo 'export DOTNET_ROOT=/root/.dotnet' >> /root/.bashrc
      export PATH=$PATH:/root/.dotnet
      export DOTNET_ROOT=/root/.dotnet

      mkdir -p /vagrant/packages
      mkdir -p ~/baget
      cd ~/baget
      wget https://github.com/loic-sharma/BaGet/releases/latest/download/BaGet.zip -O BaGet.zip
      unzip -o BaGet.zip -d BaGet
      cd BaGet

      cat <<'EOF' > appsettings.Production.json
{
  "ApiKey": "localkey",
  "Storage": { "Type": "FileSystem", "Path": "/vagrant/packages" },
  "Database": { "Type": "Sqlite", "ConnectionString": "Data Source=/vagrant/packages/baget.db" },
  "Mirror": { "Enabled": false }
}
EOF

      nohup /root/.dotnet/dotnet BaGet.dll --urls http://0.0.0.0:5557 > baget.log 2>&1 &
      BAGET_PID=$!
      echo "BaGet started with PID: $BAGET_PID"
      
      for i in {1..30}; do
        if curl -s http://127.0.0.1:5557/v3/index.json > /dev/null 2>&1; then
          echo "BaGet is ready!"
          break
        fi
        echo "Attempt $i/30: BaGet not ready yet..."
        if ! kill -0 $BAGET_PID 2>/dev/null; then
          echo "ERROR: BaGet process died! Check logs:"
          cat baget.log
          exit 1
        fi
        sleep 2
      done
      
      if ! curl -s http://127.0.0.1:5557/v3/index.json > /dev/null 2>&1; then
        echo "ERROR: BaGet failed to start. Logs:"
        cat baget.log
        exit 1
      fi
      sleep 5

      mkdir -p ~/.nuget/NuGet
      cat > ~/.nuget/NuGet/NuGet.Config <<'EOL'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="KlinikaRepo" value="http://127.0.0.1:5557/v3/index.json" protocolVersion="3" allowInsecureConnections="true" />
  </packageSources>
</configuration>
EOL

      /root/.dotnet/dotnet nuget add source http://127.0.0.1:5557/v3/index.json \
        --name KlinikaRepo \
        --configfile ~/.nuget/NuGet/NuGet.Config || true

      mkdir -p ~/klinikaapp
      cd ~/klinikaapp
      /root/.dotnet/dotnet new console -n TestApp
      cd TestApp
      /root/.dotnet/dotnet add package KlinikaApp --version 0.0.1 --source http://127.0.0.1:5557/v3/index.json || echo "Package not found"
      /root/.dotnet/dotnet restore
    SHELL
  end

end