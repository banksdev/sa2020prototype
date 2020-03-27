mkdir install_dotnet
cd install_dotnet
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
add-apt-repository universe
apt-get update
apt-get install apt-transport-https
apt-get update
apt-get install dotnet-sdk-3.1