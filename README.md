# sa2020prototype

## Prerequisites
- Ubuntu 18.04 (should also work on 16.04 and higher)
- docker engine,
- docker compose,
- docker swarm

### Setup
```bash
cd Simulator/Simulator
./install_dotnet.sh # installs .NET SDK
./init_swarm.sh
dotnet build # builds simulator project
```

## HTTP Prototype
```bash
# Build the project
cd HTTP
./build_all.sh

# Run
cd ../Simulator/Simulator
# This will run 3 tests (takes some time)
./benchmarkhttp.sh
# Results are availible in ./logs/http directory
```

## MQ Prototype
```bash
# Build the project
cd RabbitMQ
./build_all.sh

# Run
cd ../Simulator/Simulator
# This will run 3 tests (takes some time)
./benchmarkmq.sh
# Results are availible in ./logs/mq directory
```
