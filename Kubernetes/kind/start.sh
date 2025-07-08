echo "Creating Kind cluster..."
kind create cluster --name ecommerce-cluster --image kindest/node:v1.32.0

echo "Loading local images..."
kind load docker-image ecommerce/catalog-service:local-dev --name ecommerce-cluster
kind load docker-image ecommerce/notification-service:local-dev --name ecommerce-cluster
kind load docker-image ecommerce/order-service:local-dev --name ecommerce-cluster