#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status
set -o pipefail  # Catch errors in piped commands

echo "Destroying resources..."

echo "🗑️ Deleting observability namespace"
kubectl delete namespace observability || true &

echo "🗑️ Deleting ecommerce namespace"
kubectl delete namespace ecommerce || true &

echo "🗑️ Deleting ingress-nginx namespace"
kubectl delete namespace ingress-nginx || true &

wait

echo "Destroy completed successfully!"