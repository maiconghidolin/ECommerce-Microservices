#!/bin/bash

docker-compose \
    -f ../Docker/docker-compose.yml \
    -f ../CatalogService/docker-compose.yml \
    -f ../NotificationService/docker-compose.yml \
    -f ../OrderService/docker-compose.yml \
    build $@