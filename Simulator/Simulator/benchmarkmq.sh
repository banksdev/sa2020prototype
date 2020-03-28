echo "Running MQ Benchmark"

echo "Setting up swarm..."
cd ../../RabbitMQ/
./setup_swarm.sh

echo "Starting simulation.."
cd ../Simulator/Simulator
mkdir -p logs/mq
dotnet run > logs/mq/run1.log
echo "(1/3)"
dotnet run > logs/mq/run2.log
echo "(2/3)"
dotnet run > logs/mq/run3.log
echo "(3/3)"

echo "Tearing down swarm..."
cd ../../RabbitMQ/
./stack_remove.sh
