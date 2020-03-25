echo "Running MQ Benchmark"

echo "Setting up swarm..."
cd ../../RabbitMQ/
./setup_swarm.sh

echo "Starting simulation.."
cd ../Simulator/Simulator
dotnet run > logs/mq/run1.log
echo "(1/5)"
dotnet run > logs/mq/run2.log
echo "(2/5)"
dotnet run > logs/mq/run3.log
echo "(3/5)"
dotnet run > logs/mq/run4.log
echo "(4/5)"
dotnet run > logs/mq/run5.log
echo "(5/5)"

echo "Tearing down swarm..."
cd ../../RabbitMQ/
./stack_remove.sh
