#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status
set -o pipefail  # Catch errors in piped commands

echo "🔧 Applying Helmfile..."
helmfile apply

echo "📡 Applying ServiceMonitors..."
kubectl apply -f observability/service-monitors/

echo "🐘 Applying PostgreSQL manifests..."
kubectl apply -f postgres/

echo "🍃 Applying MongoDB manifests..."
kubectl apply -f mongo/

echo "📦 Deploying order-service..."
kubectl apply -f order-service/

echo "🛒 Deploying catalog-service..."
kubectl apply -f catalog-service/

echo "🔔 Deploying notification-service..."
kubectl apply -f notification-service/

echo "✅ All resources applied successfully!"