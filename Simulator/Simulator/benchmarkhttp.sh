echo "Running HTTP Benchmark"

echo "Setting up swarm..."
cd ../../HTTP/
./setup_swarm.sh

echo "Starting simulation.."
cd ../Simulator/Simulator
dotnet run > logs/http/run1.log
echo "(1/5)"
dotnet run > logs/http/run2.log
echo "(2/5)"
dotnet run > logs/http/run3.log
echo "(3/5)"
dotnet run > logs/http/run4.log
echo "(4/5)"
dotnet run > logs/http/run5.log
echo "(5/5)"

echo "Tearing down swarm..."
cd ../../HTTP/
./stack_remove.sh