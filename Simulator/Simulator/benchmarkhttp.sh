echo "Running HTTP Benchmark"

echo "Setting up swarm..."
cd ../../HTTP/
./setup_swarm.sh

echo "Starting simulation.."
cd ../Simulator/Simulator
dotnet run > logs/http/run1.log
echo "(1/3)"
dotnet run > logs/http/run2.log
echo "(2/3)"
dotnet run > logs/http/run3.log
echo "(3/3)"

echo "Tearing down swarm..."
cd ../../HTTP/
./stack_remove.sh