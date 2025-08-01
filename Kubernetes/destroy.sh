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

echo "🗑️ Deleting linkerd namespace"
kubectl delete namespace linkerd || true &

echo "🗑️ Deleting linkerd-viz namespace"
kubectl delete namespace linkerd-viz || true &

wait

echo "Destroy completed successfully!"