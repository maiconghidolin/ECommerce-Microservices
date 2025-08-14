#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status
set -o pipefail  # Catch errors in piped commands

echo "Creating namespaces..."
kubectl apply -f ../../namespace.yaml

echo "Applying CRDs..."
kubectl apply -f manifest/linkerd-edge-25.7.1-crds.yaml

echo "Applying resources..."
kubectl apply -f manifest/linkerd-edge-25.7.1.yaml

echo "Waiting for Linkerd to be ready..."
kubectl wait --for=condition=available --timeout=600s -n linkerd deployment --all

echo "Checking Linkerd status..."
linkerd check 

echo "Installing Linkerd viz..."
linkerd viz install | kubectl apply -f -

echo "Applying linkerd authorization policies..."
kubectl apply -f authorizations/server.yaml
kubectl apply -f authorizations/mesh-TLS-authentication.yaml
kubectl apply -f authorizations/authorization-policy.yaml

echo "Linkerd is now installed and ready to use."