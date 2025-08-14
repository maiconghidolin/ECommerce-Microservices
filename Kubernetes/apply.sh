#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status
set -o pipefail  # Catch errors in piped commands

echo "Creating namespaces..."
kubectl apply -f namespace.yaml

echo "🔧 Applying Helmfile CRDs..."
helmfile apply -f helmfile-crds.yaml

echo "🔧 Applying Helmfile..."
helmfile apply

echo "📡 Applying ServiceMonitors..."
kubectl apply -f observability/service-monitors/

echo "🐘 Applying PostgreSQL manifests..."
kubectl apply -f postgres/

echo "🍃 Applying MongoDB manifests..."
kubectl apply -f mongo/

echo "📦 Deploying order-service..."
helm upgrade --install order-service ./helm/order-service -n ecommerce -f ./helm/order-service/dev.values.yaml

echo "🛒 Deploying catalog-service..."
helm upgrade --install catalog-service ./helm/catalog-service -n ecommerce -f ./helm/catalog-service/dev.values.yaml

echo "🔔 Deploying notification-service..."
helm upgrade --install notification-service ./helm/notification-service -n ecommerce -f ./helm/notification-service/dev.values.yaml

echo "🌐 Applying Ingress resources..."
kubectl apply -f ingress-nginx/nginx-ingress.1.11.3.yaml

echo "⏳ Waiting for Ingress controller webhook to be ready..."
kubectl rollout status deployment ingress-nginx-controller -n ingress-nginx

echo "✅ Controller ready. Applying ecommerce ingress..."
kubectl apply -f ingress-nginx/ecommerce-ingress.yaml

echo "✅ Controller ready. Applying observability ingress..."
kubectl apply -f ingress-nginx/observability-ingress.yaml

echo "✅ All resources applied successfully!"