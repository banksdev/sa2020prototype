#!/bin/bash
docker build -f API/Dockerfile -t sa2020prototype/httpapi:latest API
docker build -f FileMShttp/Dockerfile -t sa2020prototype/httpfileservice:latest .
