#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status
set -o pipefail  # Catch errors in piped commands

echo "Creating namespaces..."
kubectl apply -f namespace.yaml

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

echo "🌐 Applying Ingress resources..."
kubectl apply -f ingress-nginx/nginx-ingress.1.11.3.yaml

echo "⏳ Waiting for Ingress controller webhook to be ready..."
kubectl rollout status deployment ingress-nginx-controller -n ingress-nginx

echo "✅ Controller ready. Applying ecommerce ingress..."
kubectl apply -f ingress-nginx/ecommerce-ingress.yaml

echo "✅ All resources applied successfully!"