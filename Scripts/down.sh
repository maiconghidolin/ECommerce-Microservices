#!/bin/bash

docker-compose \
    -p ecommerce \
    -f ../Docker/docker-compose.yml \
    -f ../CatalogService/docker-compose.yml \
    -f ../OrderService/docker-compose.yml \
    down $@