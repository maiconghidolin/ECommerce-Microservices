#!/bin/sh

kubectl create configmap featbit-sql-scripts \
  --from-file=./postgresql-scripts/ \
  -n ecommerce

kubectl apply -f init-job.yaml 

helm repo add featbit https://featbit.github.io/featbit-charts/
helm repo update

helm install featbit featbit/featbit \
  --namespace ecommerce \
  -f featbit-values.yaml

# use test@featbit.com and 123456 to connect to UI